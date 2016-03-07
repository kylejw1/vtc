using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;
using VTC.Common;

namespace VTC.Kernel
{
    /// <summary>
    /// Track hypotheses for multiple targets based on Reid's Multiple Hypothesis
    /// Tracking Algorithm
    /// </summary>
    public class MultipleHypothesisTracker
    {
        private readonly IMultipleHypothesisSettings _settings;
        private HypothesisTree _hypothesisTree = null;
        public VelocityField VelocityField { get; private set; }
        public List<TrajectoryFade> Trajectories; 

        public int ValidationRegionDeviation
        {
            get { return _settings.ValidationRegionDeviation; }
        }

        public List<Vehicle> CurrentVehicles
        {
            get
            {
                if (null == _hypothesisTree || null == _hypothesisTree.NodeData)
                    return null;

                return _hypothesisTree.NodeData.Vehicles;
            }
        }

        public List<Vehicle> DeletedVehicles
        {
            get
            {
                if (null == _hypothesisTree || null == _hypothesisTree.NodeData)
                    return null;

                return _hypothesisTree.NodeData.DeletedVehicles;
            }
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public MultipleHypothesisTracker(IMultipleHypothesisSettings settings, VelocityField velocityField)
        {
            _settings = settings;

            StateHypothesis initialHypothesis = new StateHypothesis(_settings.MissThreshold);
            _hypothesisTree = new HypothesisTree(initialHypothesis);
            _hypothesisTree.PopulateSystemDynamicsMatrices(_settings.Q_position, _settings.Q_color, _settings.R_position, _settings.R_color, _settings.Timestep, _settings.CompensationGain);

            VelocityField = velocityField;
            Trajectories = new List<TrajectoryFade>();   
        }

        /// <summary>
        /// Update targets from all child hypotheses with a new set of coordinates
        /// </summary>
        /// <param name="detections">Information about each detected item present in the latest readings.  This 
        /// list is assumed to be complete.</param>
        public void Update(Measurement[] detections, bool logState=false)
        {
            int numDetections = detections.Length;

            string logFilePath;
            System.IO.StreamWriter logfile = null;
            if (logState)
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string logsDirectoryPath = desktopPath + "\\MHTlogs\\";
                if (!Directory.Exists(logsDirectoryPath))
                    Directory.CreateDirectory(logsDirectoryPath);

                int fCount = Directory.GetFiles(logsDirectoryPath, "*", SearchOption.AllDirectories).Length;
                string logFileName = fCount.ToString() + ".txt";
                logFilePath = Path.Combine(logsDirectoryPath, logFileName);
                File.Create(logFilePath).Dispose();

                logfile = new System.IO.StreamWriter(logFilePath);
                foreach (var m in detections)
                    logfile.WriteLine("Detection: (" + m.X + ", " + m.Y + ")");

                logfile.WriteLine();
                logfile.WriteLine("MHT initial state:");
                logfile.Write(SerializeTreeState(_hypothesisTree));
            }

            //Maintain hypothesis tree
            if (_hypothesisTree.Children.Count > 0)
            {
                if (_hypothesisTree.TreeDepth() > _settings.MaxHypothesisTreeDepth)
                {
                    _hypothesisTree.Prune(1);
                    _hypothesisTree = _hypothesisTree.GetChild(0);
                }
            }

            if (logState)
            {
                logfile.WriteLine();
                logfile.WriteLine("MHT after pruning:");
                logfile.Write(SerializeTreeState(_hypothesisTree));
            }

            List<Node<StateHypothesis>> childNodeList = _hypothesisTree.GetLeafNodes();
            foreach (Node<StateHypothesis> childNode in childNodeList) //For each lowest-level hypothesis node
            {
                // Update child node
               
                if (numDetections > 0)
                {
                    GenerateChildNodes(detections, childNode);
                }
                else
                {
                    int numExistingTargets = childNode.NodeData.Vehicles.Count;
                    StateHypothesis childHypothesis = new StateHypothesis(_settings.MissThreshold);
                    childNode.AddChild(childHypothesis);
                    HypothesisTree childHypothesisTree = new HypothesisTree(childNode.Children[0].NodeData)
                    {
                        Parent = childNode
                    };
                    childHypothesisTree.PopulateSystemDynamicsMatrices(_settings.Q_position, _settings.Q_color, _settings.R_position, _settings.R_color, _settings.Timestep, _settings.CompensationGain);

                    childHypothesis.Probability = Math.Pow((1 - _settings.Pd), numExistingTargets);
                    //Update states for vehicles without Measurement
                    for (int j = 0; j < numExistingTargets; j++)
                    {
                        //Updating state for missed measurement
                        StateEstimate lastState = childNode.NodeData.Vehicles[j].StateHistory.Last();
                        StateEstimate noMeasurementUpdate = lastState.PropagateStateNoMeasurement(_settings.Timestep, _hypothesisTree.H, _hypothesisTree.R, _hypothesisTree.F, _hypothesisTree.Q, _hypothesisTree.CompensationGain);
                        childHypothesisTree.UpdateVehicleFromPrevious(j, noMeasurementUpdate, false);
                    }
                }

            }

            // Insert velocities of current vehicles into the velocity field for later use when adding new vehicles
            var pointVelocityDic = new Dictionary<Point, VelocityField.Velocity>();
            foreach (var v in CurrentVehicles)
            {
                var lastState = v.StateHistory.Last();
                var coords = new Point((int)lastState.X, (int)lastState.Y);
                var velocity = new VelocityField.Velocity(lastState.Vx, lastState.Vy);

                pointVelocityDic[coords] = velocity;
            }

            VelocityField.TryInsertVelocitiesAsync(pointVelocityDic);

            updateTrajectoriesList();

            if (logState)
            {
                logfile.WriteLine();
                logfile.WriteLine("MHT final:");
                logfile.Write(SerializeTreeState(_hypothesisTree));
                logfile.Dispose();
            }
        }

        /// <summary>
        /// Generate child nodes and fill hypotheses for a given node, using the provided measurements
        /// </summary>
        /// <param name="detections">Coordinates of all detections present in the latest measurements.  This 
        /// list is assumed to be complete.</param>
        /// <param name="hypothesisNode">The node to build the new hypotheses from</param>
        private void GenerateChildNodes(Measurement[] detections, Node<StateHypothesis> hypothesisNode)
        {
            //Allocate matrix one column for each existing vehicle plus one column for new vehicles and one for false positives, one row for each object detection event

            int numExistingTargets = hypothesisNode.NodeData.Vehicles.Count;
            int numDetections = detections.Length;

            //Got detections
            DenseMatrix falseAssignmentMatrix = DenseMatrix.Create(numDetections, numDetections, (x, y) => double.MinValue);
            double[] falseAssignmentDiagonal = Enumerable.Repeat(Math.Log10(_settings.LambdaF), numDetections).ToArray();
            falseAssignmentMatrix.SetDiagonal(falseAssignmentDiagonal); //Represents a false positive

            DenseMatrix newTargetMatrix = DenseMatrix.Create(numDetections, numDetections, (x, y) => double.MinValue);
            double[] newTargetDiagonal = Enumerable.Repeat(Math.Log10(_settings.LambdaN), numDetections).ToArray();
            newTargetMatrix.SetDiagonal(newTargetDiagonal); //Represents a new object to track

            StateEstimate[] targetStateEstimates = hypothesisNode.NodeData.GetStateEstimates();

            //Generate a matrix where each row signifies a detection and each column signifies an existing target
            //The value in each cell is the probability of the row's measurement occuring for the column's object
            DenseMatrix ambiguityMatrix = GenerateAmbiguityMatrix(detections, numExistingTargets, targetStateEstimates);

            //Generating expanded hypothesis
            //Hypothesis matrix needs to have a unique column for each detection being treated as a false positive or new object
            DenseMatrix hypothesisExpanded = GetExpandedHypothesis(
                numDetections, 
                numExistingTargets, 
                ambiguityMatrix, 
                falseAssignmentMatrix, 
                newTargetMatrix
                );

            GenerateChildHypotheses(detections, numDetections, hypothesisNode, numExistingTargets, hypothesisExpanded);
        }

        /// <summary>
        /// Fill hypotheses for childrend of a given node, using the provided measurements
        /// </summary>
        /// <param name="coords">Coordinates of all detections present in the latest readings.  This 
        /// list is assumed to be complete.</param>
        /// <param name="numDetections">Nomber of targets present in the new measurements</param>
        /// <param name="hypothesisParent">Hypothesis node to add child hypotheses to</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="hypothesisExpanded">Hypothesis matrix</param>
        private void GenerateChildHypotheses(Measurement[] coords, int numDetections, Node<StateHypothesis> hypothesisParent, int numExistingTargets, DenseMatrix hypothesisExpanded)
        {
            //Calculate K-best assignment using Murty's algorithm
            double[,] costs = hypothesisExpanded.ToArray();
            for (int i = 0; i < costs.GetLength(0); i++)
                for (int j = 0; j < costs.GetLength(1); j++)
                    costs[i, j] = -costs[i, j];

            List<int[]> kBest = OptAssign.FindKBestAssignments(costs, _settings.KHypotheses);
            int numTargetsCreated = 0;

            //Generate child hypotheses from k-best assignments
            for (int i = 0; i < kBest.Count; i++)
            {
                int[] assignment = kBest[i];
                StateHypothesis childHypothesis = new StateHypothesis(_settings.MissThreshold);
                hypothesisParent.AddChild(childHypothesis);
                HypothesisTree childHypothesisTree = new HypothesisTree(hypothesisParent.Children[i].NodeData)
                {
                    Parent = hypothesisParent
                };
                childHypothesisTree.PopulateSystemDynamicsMatrices(_settings.Q_position, _settings.Q_color, _settings.R_position, _settings.R_color, _settings.Timestep, _settings.CompensationGain);

                childHypothesis.Probability = OptAssign.assignmentCost(costs, assignment);
                //Update states for vehicles without measurements
                for (int j = 0; j < numExistingTargets; j++)
                {
                    //If this target is not detected
                    if (!(assignment.Contains(j + numDetections)))
                    {
                        //Updating state for missed measurement
                        StateEstimate lastState = hypothesisParent.NodeData.Vehicles[j].StateHistory.Last();
                        StateEstimate noMeasurementUpdate = lastState.PropagateStateNoMeasurement(_settings.Timestep, _hypothesisTree.H, _hypothesisTree.R, _hypothesisTree.F, _hypothesisTree.Q, _hypothesisTree.CompensationGain);
                        childHypothesisTree.UpdateVehicleFromPrevious(j, noMeasurementUpdate, false);
                    }
                }

                for (int j = 0; j < numDetections; j++)
                {

                    //Account for new vehicles
                    if (assignment[j] >= numExistingTargets + numDetections && numExistingTargets + numTargetsCreated < _settings.MaxTargets) //Add new vehicle
                    {
                        // Find predicted velocity
                        var velocity = VelocityField.GetAvgVelocity((int)coords[j].X, (int)coords[j].Y);
                        
                        //Creating new vehicle
                        numTargetsCreated++;
                        childHypothesis.AddVehicle(
                            Convert.ToInt16(coords[j].X), 
                            Convert.ToInt16(coords[j].Y), 
                            velocity.v_x, 
                            velocity.v_y,
                            Convert.ToInt16(coords[j].Red),
                            Convert.ToInt16(coords[j].Green),
                            Convert.ToInt16(coords[j].Blue), 
                            Turn.Unknown,
                            _settings.VehicleInitialCovX,
                            _settings.VehicleInitialCovY,
                            _settings.VehicleInitialCovVX,
                            _settings.VehicleInitialCovVY,
                            _settings.VehicleInitialCovR,
                            _settings.VehicleInitialCovG,
                            _settings.VehicleInitialCovB);

                    }
                    else if (assignment[j] >= numDetections && assignment[j] < numDetections + numExistingTargets) //Update states for vehicles with measurements
                    {
                        //Updating vehicle with measurement
                        StateEstimate lastState = hypothesisParent.NodeData.Vehicles[assignment[j] - numDetections].StateHistory.Last();
                        StateEstimate estimatedState = lastState.PropagateState(_settings.Timestep, _hypothesisTree.H, _hypothesisTree.R, _hypothesisTree.F, _hypothesisTree.Q, coords[j]);
                        childHypothesisTree.UpdateVehicleFromPrevious(assignment[j] - numDetections, estimatedState, true);
                    }

                }
            }
        }

        /// <summary>
        /// Generate the hypothesis matrix
        /// </summary>
        /// <param name="numDetections">Number of targets detected in current measurements</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="ambiguityMatrix">Ambiguity matrix containing probability that a given measurement belongs to a given target</param>
        /// <param name="falseAssignmentMatrix">Probability matrix if false assignment</param>
        /// <param name="newTargetMatrix">Probability matrix indicating likelihood that a measurement is from a new target</param>
        /// <returns></returns>
        private static DenseMatrix GetExpandedHypothesis(int numDetections, int numExistingTargets, DenseMatrix ambiguityMatrix, DenseMatrix falseAssignmentMatrix, DenseMatrix newTargetMatrix)
        {
            DenseMatrix hypothesisExpanded;
            if (numExistingTargets > 0)
            {
                //Expanded hypothesis: targets exist
                DenseMatrix targetAssignmentMatrix = (DenseMatrix)ambiguityMatrix.SubMatrix(0, numDetections, 1, numExistingTargets);
                hypothesisExpanded = new DenseMatrix(numDetections, 2 * numDetections + numExistingTargets);
                hypothesisExpanded.SetSubMatrix(0, numDetections, 0, numDetections, falseAssignmentMatrix);
                hypothesisExpanded.SetSubMatrix(0, numDetections, numDetections, numExistingTargets, targetAssignmentMatrix);
                hypothesisExpanded.SetSubMatrix(0, numDetections, numDetections + numExistingTargets, numDetections, newTargetMatrix);
            }
            else
            {
                //Expanded hypothesis: no targets
                hypothesisExpanded = new DenseMatrix(numDetections, 2 * numDetections);
                hypothesisExpanded.SetSubMatrix(0, numDetections, 0, numDetections, falseAssignmentMatrix);
                hypothesisExpanded.SetSubMatrix(0, numDetections, numDetections, numDetections, newTargetMatrix);
            }
            return hypothesisExpanded;
        }

        /// <summary>
        /// Generate a matrix where each row signifies a detection and each column signifies an existing target
        /// The value in each cell is the probability of the row's measurement occuring for the column's object
        /// </summary>
        /// <param name="coordinates">New measurements</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="targetStateEstimates">Latest state estimates for each known target</param>
        /// <returns></returns>
        private DenseMatrix GenerateAmbiguityMatrix(Measurement[] coordinates, int numExistingTargets, StateEstimate[] targetStateEstimates)
        {
            // TODO:  Can't we get numExistingTargets from target_state_estimates Length?

            int numDetections = coordinates.Length;
            var ambiguityMatrix = new DenseMatrix(numDetections, numExistingTargets + 2);
            Normal norm = new Normal();
            
            for (int i = 0; i < numExistingTargets; i++)
            {
                //Get this car's estimated next position using Kalman predictor
                StateEstimate noMeasurementEstimate = targetStateEstimates[i].PropagateStateNoMeasurement(_settings.Timestep, _hypothesisTree.H, _hypothesisTree.R, _hypothesisTree.F, _hypothesisTree.Q, _hypothesisTree.CompensationGain);

                DenseMatrix pBar = new DenseMatrix(7, 7);
                pBar[0, 0] = noMeasurementEstimate.CovX;
                pBar[1, 1] = noMeasurementEstimate.CovVx;
                pBar[2, 2] = noMeasurementEstimate.CovY;
                pBar[3, 3] = noMeasurementEstimate.CovVy;
                pBar[4, 4] = noMeasurementEstimate.CovRed;
                pBar[5, 5] = noMeasurementEstimate.CovGreen;
                pBar[6, 6] = noMeasurementEstimate.CovBlue;

                DenseMatrix hTrans = (DenseMatrix)_hypothesisTree.H.Transpose();
                DenseMatrix b = _hypothesisTree.H * pBar * hTrans + _hypothesisTree.R;
                DenseMatrix bInverse = (DenseMatrix)b.Inverse();

                for (int j = 0; j < numDetections; j++)
                {
                    DenseMatrix zMeas = new DenseMatrix(5, 1);
                    zMeas[0, 0] = coordinates[j].X;
                    zMeas[1, 0] = coordinates[j].Y;
                    zMeas[2, 0] = coordinates[j].Red;
                    zMeas[3, 0] = coordinates[j].Green;
                    zMeas[4, 0] = coordinates[j].Blue;

                    DenseMatrix zEst = new DenseMatrix(5, 1);
                    zEst[0, 0] = noMeasurementEstimate.X;
                    zEst[1, 0] = noMeasurementEstimate.Y;
                    zEst[2, 0] = noMeasurementEstimate.Red;
                    zEst[3, 0] = noMeasurementEstimate.Green;
                    zEst[4, 0] = noMeasurementEstimate.Blue;

                    DenseMatrix residual = StateEstimate.Residual(zEst, zMeas);
                    DenseMatrix residualTranspose = (DenseMatrix)residual.Transpose();
                    DenseMatrix mahalanobis = residualTranspose * bInverse * residual;
                    double mahalanobisDistance = Math.Sqrt(mahalanobis[0, 0]);

                    if (mahalanobisDistance > ValidationRegionDeviation)
                        ambiguityMatrix[j, i + 1] = Double.MinValue;
                    else
                    {
                        ambiguityMatrix[j, i + 1] = Math.Log10(_settings.Pd * norm.Density(mahalanobisDistance) / (1 - _settings.Pd));
                    }
                }
            }
            return ambiguityMatrix;
        }

        private void updateTrajectoriesList()
        {
            //Eliminate stale trajectories
            foreach (var t in Trajectories.ToList())
                if (t.exitTime < DateTime.Now - TimeSpan.FromSeconds(3))
                    Trajectories.Remove(t);

            //Add new trajectories
            foreach (var v in DeletedVehicles)
            {
                var t = new TrajectoryFade();
                t.exitTime = DateTime.Now;
                t.stateEstimates = v.StateHistory;
                Trajectories.Add(t);
            }
        }

        public string SerializeTreeState(HypothesisTree t)
        {
            string tree_state = "========MHT=======" + Environment.NewLine;
            foreach (var h in t.ToList())
            {
                tree_state += "Node depth " + h.NodeDepth() + Environment.NewLine;
                tree_state += SerializeNodeState(h.NodeData);
            }
            return tree_state;
        }

        public string SerializeNodeState(StateHypothesis h)
        {
            string node_state = " -----Node------" + Environment.NewLine;
            node_state += " p: " + h.Probability + Environment.NewLine;
            
            if(h.Vehicles.Count > 0)
            {
                node_state += " Vehicles (all): " + Environment.NewLine;
                foreach (var v in h.Vehicles)
                    node_state += SerializeTrackerState(v);
            }
            
            if(h.NewVehicles.Count > 0)
            {
                node_state += " Vehicles (new): " + Environment.NewLine;
                foreach (var v in h.NewVehicles)
                    node_state += SerializeTrackerState(v);
            }
            
            if(h.DeletedVehicles.Count > 0)
            {
                node_state += " Vehicles (deleted): " + Environment.NewLine;
                foreach (var v in h.DeletedVehicles)
                    node_state += SerializeTrackerState(v);
            }

            return node_state;
        }

        public string SerializeTrackerState(Vehicle v)
        {
            StateEstimate latest = v.StateHistory.Last();
            string tracker_state = "  Tracker (" + latest.X + "," + latest.Y + "), " + latest.MissedDetections + " misses" + Environment.NewLine;
            return tracker_state;
        }
    }

    public struct TrajectoryFade
    {
        public DateTime exitTime;
        public List<StateEstimate> stateEstimates;
    }
}

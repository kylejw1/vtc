using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using VTC.Kernel.Settings;

namespace VTC.Kernel
{
    /// <summary>
    /// Track hypotheses for multiple targets based on Reid's Multiple Hypothesis
    /// Tracking Algorithm
    /// </summary>
    public class MultipleHypothesisTracker
    {
        private readonly IMultipleHypothesisSettings _settings;
        private HypothesisTree HypothesisTree = null;

        public int ValidationRegionDeviation
        {
            get { return _settings.ValidationRegionDeviation; }
        }

        public List<Vehicle> CurrentVehicles
        {
            get
            {
                if (null == HypothesisTree || null == HypothesisTree.nodeData)
                    return null;

                return HypothesisTree.nodeData.vehicles;
            }
        }

        public List<Vehicle> DeletedVehicles
        {
            get
            {
                if (null == HypothesisTree || null == HypothesisTree.nodeData)
                    return null;

                return HypothesisTree.nodeData.deleted_vehicles;
            }
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public MultipleHypothesisTracker(IMultipleHypothesisSettings settings)
        {
            _settings = settings;

            StateHypothesis initialHypothesis = new StateHypothesis(_settings.MissThreshold);
            HypothesisTree = new HypothesisTree(initialHypothesis);
        }

        /// <summary>
        /// Update targets from all child hypotheses with a new set of coordinates
        /// </summary>
        /// <param name="detections">Information about each detected item present in the latest readings.  This 
        /// list is assumed to be complete.</param>
        public void Update(Measurements[] detections)
        {
            int numDetections = detections.Length;

            //Maintain hypothesis tree
            if (HypothesisTree.children.Count > 0)
            {
                if (HypothesisTree.TreeDepth() > _settings.MaxHypothesisTreeDepth)
                {
                    HypothesisTree.Prune(1);
                    HypothesisTree = HypothesisTree.GetChild(0);
                }

                //To do: save deleted
                //hypothesis_tree.SaveDeleted(file path, length threshold);
            }

            List<Node<StateHypothesis>> childNodeList = HypothesisTree.GetLeafNodes();
            foreach (Node<StateHypothesis> childNode in childNodeList) //For each lowest-level hypothesis node
            {
                // Update child node
               
                if (numDetections > 0)
                {
                    GenerateChildNodes(detections, childNode);
                }
                else
                {
                    int numExistingTargets = childNode.nodeData.vehicles.Count;
                    StateHypothesis child_hypothesis = new StateHypothesis(_settings.MissThreshold);
                    childNode.AddChild(child_hypothesis);
                    HypothesisTree child_hypothesis_tree = new HypothesisTree(childNode.children[0].nodeData);
                    child_hypothesis_tree.parent = childNode;

                    child_hypothesis.probability = Math.Pow((1 - _settings.Pd), numExistingTargets);
                    //Update states for vehicles without measurements
                    for (int j = 0; j < numExistingTargets; j++)
                    {
                        //Updating state for missed measurement
                        StateEstimate last_state = childNode.nodeData.vehicles[j].state_history.Last();
                        StateEstimate no_measurement_update = last_state.PropagateStateNoMeasurement(0.033, HypothesisTree.H, HypothesisTree.R, HypothesisTree.F, HypothesisTree.Q, HypothesisTree.compensation_gain);
                        child_hypothesis_tree.UpdateVehicleFromPrevious(j, no_measurement_update, false);
                    }
                }

            }

        }

        /// <summary>
        /// Generate child nodes and fill hypotheses for a given node, using the provided measurements
        /// </summary>
        /// <param name="detections">Coordinates of all detections present in the latest measurements.  This 
        /// list is assumed to be complete.</param>
        /// <param name="hypothesisNode">The node to build the new hypotheses from</param>
        private void GenerateChildNodes(Measurements[] detections, Node<StateHypothesis> hypothesisNode)
        {
            //Allocate matrix one column for each existing vehicle plus one column for new vehicles and one for false positives, one row for each object detection event

            int numExistingTargets = hypothesisNode.nodeData.vehicles.Count;
            int numDetections = detections.Length;

            //Got detections
            DenseMatrix false_assignment_matrix = new DenseMatrix(numDetections, numDetections, Double.MinValue);
            double[] false_assignment_diagonal = Enumerable.Repeat(Math.Log10(_settings.LambdaF), numDetections).ToArray();
            false_assignment_matrix.SetDiagonal(false_assignment_diagonal); //Represents a false positive

            DenseMatrix new_target_matrix = new DenseMatrix(numDetections, numDetections, Double.MinValue);
            double[] new_target_diagonal = Enumerable.Repeat(Math.Log10(_settings.LambdaN), numDetections).ToArray();
            new_target_matrix.SetDiagonal(new_target_diagonal); //Represents a new object to track

            StateEstimate[] target_state_estimates = hypothesisNode.nodeData.GetStateEstimates();

            //Generate a matrix where each row signifies a detection and each column signifies an existing target
            //The value in each cell is the probability of the row's measurement occuring for the column's object
            DenseMatrix ambiguity_matrix = GenerateAmbiguityMatrix(detections, numExistingTargets, target_state_estimates);

            //Generating expanded hypothesis
            //Hypothesis matrix needs to have a unique column for each detection being treated as a false positive or new object
            DenseMatrix hypothesis_expanded = GetExpandedHypothesis(
                numDetections, 
                numExistingTargets, 
                ambiguity_matrix, 
                false_assignment_matrix, 
                new_target_matrix
                );

            GenerateChildHypotheses(detections, numDetections, hypothesisNode, numExistingTargets, hypothesis_expanded);
        }

        /// <summary>
        /// Fill hypotheses for childrend of a given node, using the provided measurements
        /// </summary>
        /// <param name="coords">Coordinates of all detections present in the latest readings.  This 
        /// list is assumed to be complete.</param>
        /// <param name="numDetections">Nomber of targets present in the new measurements</param>
        /// <param name="hypothesisParent">Hypothesis node to add child hypotheses to</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="hypothesis_expanded">Hypothesis matrix</param>
        private void GenerateChildHypotheses(Measurements[] coords, int numDetections, Node<StateHypothesis> hypothesisParent, int numExistingTargets, DenseMatrix hypothesis_expanded)
        {
            //Calculate K-best assignment using Murty's algorithm
            double[,] costs = hypothesis_expanded.ToArray();
            //Console.WriteLine("Finding k-best assignment");
            for (int i = 0; i < costs.GetLength(0); i++)
                for (int j = 0; j < costs.GetLength(1); j++)
                    costs[i, j] = -costs[i, j];

            List<int[]> k_best = OptAssign.FindKBestAssignments(costs, _settings.KHypotheses);

            //Generate child hypotheses from k-best assignments
            for (int i = 0; i < k_best.Count; i++)
            {
                int[] assignment = k_best[i];
                StateHypothesis child_hypothesis = new StateHypothesis(_settings.MissThreshold);
                hypothesisParent.AddChild(child_hypothesis);
                HypothesisTree child_hypothesis_tree = new HypothesisTree(hypothesisParent.children[i].nodeData);
                child_hypothesis_tree.parent = hypothesisParent;

                child_hypothesis.probability = OptAssign.assignmentCost(costs, assignment);
                //Update states for vehicles without measurements
                for (int j = 0; j < numExistingTargets; j++)
                {
                    //If this target is not detected
                    if (!(assignment.Contains(j + numDetections)))
                    {
                        //Updating state for missed measurement
                        StateEstimate last_state = hypothesisParent.nodeData.vehicles[j].state_history.Last();
                        StateEstimate no_measurement_update = last_state.PropagateStateNoMeasurement(0.033, HypothesisTree.H, HypothesisTree.R, HypothesisTree.F, HypothesisTree.Q, HypothesisTree.compensation_gain);
                        child_hypothesis_tree.UpdateVehicleFromPrevious(j, no_measurement_update, false);
                    }
                }

                for (int j = 0; j < numDetections; j++)
                {

                    //Account for new vehicles
                    if (assignment[j] >= numExistingTargets + numDetections && numExistingTargets < _settings.MaxTargets) //Add new vehicle
                    {
                        //Creating new vehicle
                        child_hypothesis.AddVehicle(
                            Convert.ToInt16(coords[j].x), 
                            Convert.ToInt16(coords[j].y), 
                            0, 
                            0,
                            Convert.ToInt16(coords[j].red),
                            Convert.ToInt16(coords[j].green),
                            Convert.ToInt16(coords[j].blue));

                    }
                    else if (assignment[j] >= numDetections && assignment[j] < numDetections + numExistingTargets) //Update states for vehicles with measurements
                    {
                        //Updating vehicle with measurement
                        StateEstimate last_state = hypothesisParent.nodeData.vehicles[assignment[j] - numDetections].state_history.Last();
                        StateEstimate estimated_state = last_state.PropagateState(0.033, HypothesisTree.H, HypothesisTree.R, HypothesisTree.F, HypothesisTree.Q, coords[j]);
                        child_hypothesis_tree.UpdateVehicleFromPrevious(assignment[j] - numDetections, estimated_state, true);
                    }

                }
            }
        }

        /// <summary>
        /// Generate the hypothesis matrix
        /// </summary>
        /// <param name="numDetections">Number of targets detected in current measurements</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="ambiguity_matrix">Ambiguity matrix containing probability that a given measurement belongs to a given target</param>
        /// <param name="false_assignment_matrix">Probability matrix if false assignment</param>
        /// <param name="new_target_matrix">Probability matrix indicating likelihood that a measurement is from a new target</param>
        /// <returns></returns>
        private static DenseMatrix GetExpandedHypothesis(int numDetections, int numExistingTargets, DenseMatrix ambiguity_matrix, DenseMatrix false_assignment_matrix, DenseMatrix new_target_matrix)
        {
            DenseMatrix hypothesis_expanded;
            if (numExistingTargets > 0)
            {
                //Expanded hypothesis: targets exist
                DenseMatrix target_assignment_matrix = (DenseMatrix)ambiguity_matrix.SubMatrix(0, numDetections, 1, numExistingTargets);

                hypothesis_expanded = new DenseMatrix(numDetections, 2 * numDetections + numExistingTargets);

                hypothesis_expanded.SetSubMatrix(0, numDetections, 0, numDetections, false_assignment_matrix);
                hypothesis_expanded.SetSubMatrix(0, numDetections, numDetections, numExistingTargets, target_assignment_matrix);
                hypothesis_expanded.SetSubMatrix(0, numDetections, numDetections + numExistingTargets, numDetections, new_target_matrix);

            }
            else
            {
                //Expanded hypothesis: no targets
                hypothesis_expanded = new DenseMatrix(numDetections, 2 * numDetections);
                hypothesis_expanded.SetSubMatrix(0, numDetections, 0, numDetections, false_assignment_matrix);
                hypothesis_expanded.SetSubMatrix(0, numDetections, numDetections, numDetections, new_target_matrix);
            }
            return hypothesis_expanded;
        }

        /// <summary>
        /// Generate a matrix where each row signifies a detection and each column signifies an existing target
        /// The value in each cell is the probability of the row's measurement occuring for the column's object
        /// </summary>
        /// <param name="coordinates">New measurements</param>
        /// <param name="numExistingTargets">Number of currently detected targets</param>
        /// <param name="target_state_estimates">Latest state estimates for each known target</param>
        /// <returns></returns>
        private DenseMatrix GenerateAmbiguityMatrix(Measurements[] coordinates, int numExistingTargets, StateEstimate[] target_state_estimates)
        {
            // TODO:  Can't we get numExistingTargets from target_state_estimates Length?

            DenseMatrix ambiguity_matrix;
            int num_detections = coordinates.Length;
            ambiguity_matrix = new DenseMatrix(num_detections, numExistingTargets + 2);
            Normal norm = new Normal();

            for (int i = 0; i < numExistingTargets; i++)
            {
                //Get this car's estimated next position using Kalman predictor
                StateEstimate no_measurement_estimate = target_state_estimates[i].PropagateStateNoMeasurement(0.033, HypothesisTree.H, HypothesisTree.R, HypothesisTree.F, HypothesisTree.Q, HypothesisTree.compensation_gain);

                DenseMatrix P_bar = new DenseMatrix(7, 7);
                P_bar[0, 0] = no_measurement_estimate.cov_x;
                P_bar[1, 1] = no_measurement_estimate.cov_vx;
                P_bar[2, 2] = no_measurement_estimate.cov_y;
                P_bar[3, 3] = no_measurement_estimate.cov_vy;
                P_bar[4, 4] = no_measurement_estimate.cov_red;
                P_bar[5, 5] = no_measurement_estimate.cov_green;
                P_bar[6, 6] = no_measurement_estimate.cov_blue;

                DenseMatrix H_trans = (DenseMatrix)HypothesisTree.H.Transpose();
                DenseMatrix B = HypothesisTree.H * P_bar * H_trans + HypothesisTree.R;
                DenseMatrix B_inverse = (DenseMatrix)B.Inverse();

                for (int j = 0; j < num_detections; j++)
                {
                    DenseMatrix z_meas = new DenseMatrix(5, 1);
                    z_meas[0, 0] = coordinates[j].x;
                    z_meas[1, 0] = coordinates[j].y;
                    z_meas[2, 0] = coordinates[j].red;
                    z_meas[3, 0] = coordinates[j].green;
                    z_meas[4, 0] = coordinates[j].blue;

                    DenseMatrix z_est = new DenseMatrix(5, 1);
                    z_est[0, 0] = no_measurement_estimate.x;
                    z_est[1, 0] = no_measurement_estimate.y;
                    z_est[2, 0] = no_measurement_estimate.red;
                    z_est[3, 0] = no_measurement_estimate.green;
                    z_est[4, 0] = no_measurement_estimate.blue;

                    DenseMatrix residual = StateEstimate.residual(z_est, z_meas);
                    DenseMatrix residual_transpose = (DenseMatrix)residual.Transpose();
                    DenseMatrix mahalanobis = residual_transpose * B_inverse * residual;
                    double mahalanobis_distance = Math.Sqrt(mahalanobis[0, 0]);

                    if (mahalanobis_distance > ValidationRegionDeviation)
                        ambiguity_matrix[j, i + 1] = Double.MinValue;
                    else
                    {
                        ambiguity_matrix[j, i + 1] = Math.Log10(_settings.Pd * norm.Density(mahalanobis_distance) / (1 - _settings.Pd));
                    }
                }
            }
            return ambiguity_matrix;
        }
    }
}

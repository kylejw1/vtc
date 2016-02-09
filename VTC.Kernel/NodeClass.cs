using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using VTC.Common;

//using Excel = Microsoft.Office.Interop.Excel;

//TreeLib
//A tree class with inherited class HypothesisTree for use in Multiple Hypothesis Tracking algorithm. 
//This code is intended for use in traffic monitoring using a single video camera. 
//
//Written by Alexander Farley 
//alexander.farley@utoronto.ca
//Tested and compiled in VS2010 for .NET 4 framework
//Started October 24 2011
//Updated October 27 2011 - debugging
//Updated Dec 6 2011 - added initial angle, path length, is_pedestrian states to StateHypothesis struct
//----------------------------------------------------------------------
namespace VTC.Kernel
{

    public enum MeasurementSource { FalsePositive, ExistingVehicle, NewVehicle }; 

    public class Node<T>
    {
        public Node<T> Parent;
        public List<Node<T>> Children;

        public T NodeData; 

        public Node(T value) : this()
        {
            NodeData = value;
        }

        public Node()
        {
            Children = new List<Node<T>>();
        }

        public Node<T> GetRoot()
        {            
            
            Func<Node<T>, Node<T>> getParent = null;
            getParent = x =>
            {
                if (x.Parent != null) { return getParent(x.Parent); } else { return this; }
            };

            return getParent(this);
        }

        public virtual void AddChild(T value)
        {
            Node<T> newchild = new Node<T>(value) {Parent = this};
            if (Children != null)
                Children.Add(newchild);
        }

        //Recursive function used in GetChain to iterate upwards through tree pushing each new node to the front of a list 
        private List<Node<T>> AddBack(List<Node<T>> x)
        {
        if (x[0].Parent != null)
                {
                    x.Add(x[0].Parent); //If parent node exists, add to start of the SortedList 
                    AddBack(x); //Continue process with parent node
                }
        return x;
        }

        /// <summary>
        /// Return list of nodes starting from root to this node
        /// </summary>
        /// <returns></returns>
        public List<Node<T>> PathFromRoot() 
        {
            List<Node<T>> pathList = new List<Node<T>> {this};
            return AddBack(pathList);
        }

        /// <summary>
        /// Return list containing all nodes in this tree
        /// </summary>
        public List<Node<T>> ToList()
        {
            List<Node<T>> list = new List<Node<T>>();
            Node<T> root = GetRoot();
            Action<Node<T>> traverse = null;
            traverse = x => { list.Add(x); x.Children.ForEach(traverse); };
            traverse(root);
            return list;
        }

        /// <summary>
        /// Return list containing all leaf nodes in this tree
        /// </summary>
        public List<Node<T>> GetLeafNodes()
        {
            var list = new List<Node<T>>();
            var root = GetRoot();
            Action<Node<T>> traverseLeafnodes = null;
            traverseLeafnodes = x => {
               if (x.Children == null || x.Children.Count == 0) //Not sure why checking against null doesn't work here, maybe IsNull is not implemented. Why isn't this problem encountered in other places?
                   list.Add(x); 
               else
                x.Children.ForEach(traverseLeafnodes); 
            };
            traverseLeafnodes(root);
            return list;
        }

        //Get number of nodes from root to leaf nodes, following "first child" path. This assumes that all branches are of equal depth. 
        public int TreeDepth()
        {
            var depth = TraverseDepth(this);
            return depth;
        }

        private static int TraverseDepth(Node<T> inputNode)
        { 
            var depth = 0;
            if (inputNode.Children.Count > 0)
                depth = inputNode.Children.Select(TraverseDepth).Concat(new[] {depth}).Max();

            return depth+1;
        }

        //Get this node's depth
        public int NodeDepth()
        {
            var nodeDepth = 1;
            var currentNode = this;
                    while (currentNode.Parent != null)
                {
                    currentNode = currentNode.Parent;
                    nodeDepth++;
                }
            return nodeDepth;
        }

        //Return array populated with # of nodes at each depth 
        public int[] NodeCountByDepth()
        {
            var treeDepth = TreeDepth();
            var depthArray = new int[treeDepth];
            var nodeList = ToList();
            foreach (var thisNode in nodeList)
                depthArray[thisNode.NodeDepth() - 1]++;

            return depthArray;
        }

        public int NumChildren()
        {
            var numChildren = 0;
            Action<Node<T>> traverse = null;
            traverse = x =>
            {
                numChildren++;
                if (x.Children == null || x.Children.Count == 0) //Not sure why checking against null doesn't work here, maybe IsNull is not implemented. Why isn't this problem encountered in other places?
                    return;
                x.Children.ForEach(traverse);
            };
            traverse(this);
            return numChildren-1;
        }

    }


    /// <summary>
    /// Object containing data-measurement association, associate state estimates and all other associated probablities and esimates
    /// necessary for multiple hypothesis tracking. 
    /// </summary>
    public class StateHypothesis
    {
        public double Probability;

        public bool[,] AssignmentMatrix;

        public List<Vehicle> DeletedVehicles;

        public List<Vehicle> Vehicles;

        public List<Vehicle> NewVehicles;

        public int MissDetectionThreshold;

        public StateHypothesis(int missThreshold)
        {
            Probability = 1;
            AssignmentMatrix = new bool[0,0];
            Vehicles = new List<Vehicle>();
            DeletedVehicles = new List<Vehicle>();
            NewVehicles = new List<Vehicle>();
            MissDetectionThreshold = missThreshold;
        }

        public StateHypothesis(double initialProbablity, int numVehicles, int numMeasurements,int missThreshold)
        {
            Probability = initialProbablity;
            AssignmentMatrix = new bool[numMeasurements, numVehicles + 2];
            DeletedVehicles = new List<Vehicle>();
            Vehicles = new List<Vehicle>(numVehicles);
            MissDetectionThreshold = missThreshold;
        }

        public StateEstimate[] GetStateEstimates()
        {
            StateEstimate[] vehicleStateEstimates = new StateEstimate[Vehicles.Count];
            for (int i = 0; i < Vehicles.Count; i++)
                vehicleStateEstimates[i] = Vehicles[i].StateHistory[Vehicles[i].StateHistory.Count - 1];
            
            return vehicleStateEstimates;
        }

        public StateEstimate[,] GetDeletedStateEstimates()
        {
            StateEstimate[,] deletedVehicleStateEstimates = new StateEstimate[DeletedVehicles.Count,2];
            for (int i = 0; i < DeletedVehicles.Count; i++)
            {
                try
                {
                    deletedVehicleStateEstimates[i, 0] = DeletedVehicles[i].StateHistory[0];
                }
                catch (Exception)
                {
                    throw new ArgumentException("Initial state history missing");
                }

                try
                {
                    deletedVehicleStateEstimates[i, 1] = DeletedVehicles[i].StateHistory[DeletedVehicles[i].StateHistory.Count - 1];
                }
                catch (Exception)
                {
                    var errorString = "Final state history missing. History length: " + DeletedVehicles[i].StateHistory.Count;
                    throw new ArgumentException(errorString);
                }
            }

            return deletedVehicleStateEstimates;
        }

        public StateEstimate[] GetNewStateEstimates()
        {
            var newVehicleStateEstimates = new StateEstimate[NewVehicles.Count];
            for (var i = 0; i < NewVehicles.Count; i++)
                newVehicleStateEstimates[i] = NewVehicles[i].StateHistory[NewVehicles[i].StateHistory.Count-1];

            return newVehicleStateEstimates;
        }

        public void AddVehicle(int x, int y, double vx, double vy, int red, int green, int blue, Turn turn, double covx, double covy, double covVx, double covVy, double covR, double covG, double covB)
        {
            var initialState = new StateEstimate
            {
                X = x,
                Y = y,
                Red = red,
                Green = green,
                Blue = blue,
                Vx = vx,
                Vy = vy,
                IsPedestrian = false,
                Turn = turn,
                CovX = covx,
                CovY = covy,
                CovVx = covVx,
                CovVy = covVy,
                CovRed = covR,
                CovGreen = covG,
                CovBlue = covB
            };


            var newVehicle = new Vehicle(initialState);
            Vehicles.Add(newVehicle);
            NewVehicles.Add(newVehicle);
        }
    }

    /// <summary>
    /// Main structure containing hypothesis tree for Multiple Hypothesis Tracking algorithm
    /// </summary>
    public class HypothesisTree : Node<StateHypothesis> 
    {

        public DenseMatrix H; //Measurement equation
        public DenseMatrix P; //
        public DenseMatrix F; //Motion equation
        public DenseMatrix Q; //Covariance
        public DenseMatrix R; //

        public double CompensationGain; //Gain applied to process noise when a measurement is missed

        public HypothesisTree(StateHypothesis value) : base(value)
        {
        }


        // ************************************************ //
        // *************** System Dynamics: *************** //
        // ************************************************ //
        //  x_new  = x_old + dt*vx;
        //  vy_new = vy_old
        //  y_new  = y_old + dt*vy
        //  vx_new = vx_old
        //  R_new = R_old
        //  G_new = G_old
        //  B_new = B_old
        // ************************************************ //
        public void PopulateSystemDynamicsMatrices(double qPosition, double qColor, double rPosition, double rColor, double dt, double compensationGain)
        {
            H = new DenseMatrix(5, 7); // Measurement equation: x,y,R,G,B are observed (not velocities)
            H[0, 0] = 1;
            H[1, 2] = 1;
            H[2, 4] = 1;
            H[3, 5] = 1;
            H[4, 6] = 1;

            F = new DenseMatrix(7, 7); //Motion equation
            F[0, 0] = 1;
            F[0, 1] = dt;
            F[1, 1] = 1;
            F[2, 2] = 1;
            F[2, 3] = dt;
            F[3, 3] = 1;
            F[4, 4] = 1;
            F[5, 5] = 1;
            F[6, 6] = 1;

            Q = new DenseMatrix(7, 7); //Process covariance
            Q[0, 0] = (dt * dt * dt * dt / 4) * qPosition;  
            Q[0, 1] = (dt * dt * dt / 3) * qPosition;
            Q[1, 0] = (dt * dt * dt / 3) * qPosition;
            Q[1, 1] = (dt * dt / 2) * qPosition;
            Q[2, 2] = (dt * dt * dt * dt / 4) * qPosition;
            Q[2, 3] = (dt * dt * dt / 3) * qPosition;
            Q[3, 2] = (dt * dt * dt / 3) * qPosition;
            Q[3, 3] = (dt * dt / 2) * qPosition;
            Q[4, 4] = qColor; 
            Q[5, 5] = qColor;
            Q[6, 6] = qColor;

            R = new DenseMatrix(5, 5); //Measurement covariance
            R[0, 0] = rPosition;
            R[1, 1] = rPosition;
            R[2, 2] = rColor;
            R[3, 3] = rColor;
            R[4, 4] = rColor;

            CompensationGain = compensationGain;
        }

        /// <summary>
        /// Create new child hypothesis
        /// </summary>
        /// <param name="value">New child's StateHypothesis</param>
        public override void AddChild(StateHypothesis value)
        {
            var newchild = new HypothesisTree(value) {Parent = this};
            if (Children != null)
                Children.Add(newchild);
        }

        /// <summary>
        /// Copies & updates state history of a vehicle from parent node referenced by integer
        /// </summary>
        /// <param name="address">Index of vehicle to be updated in parent StateHypothesis</param>
        /// <param name="currentState">New state of vehicle to be updated</param>
        /// <param name="withMeasurement">True if the new state comes from a measurement update; false if the new state comes from a pure prediction. </param>
        public void UpdateVehicleFromPrevious(int address, StateEstimate currentState, bool withMeasurement)
        {
            var parentHypothesis = Parent.NodeData;
            var lastFrameVehicle = parentHypothesis.Vehicles[address];
            currentState.IsPedestrian = lastFrameVehicle.StateHistory[lastFrameVehicle.StateHistory.Count - 1].IsPedestrian;
            var updatedVehicle = new Vehicle(lastFrameVehicle.StateHistory, currentState);
            if (!withMeasurement)
                currentState.MissedDetections++;
            else
                currentState.MissedDetections = 0; 

            if (currentState.MissedDetections < NodeData.MissDetectionThreshold)
                NodeData.Vehicles.Add(updatedVehicle);
            else
                NodeData.DeletedVehicles.Add(updatedVehicle);
        }


        /// <summary>
        /// Compare probability of two hypotheses
        /// </summary>
        /// <param name="x">Reference to first node</param>
        /// <param name="y">Refernce to second node</param>
        /// <returns>1 if p(a)>p(b), 0 otherwise</returns>
        private static int ProbCompare(Node<StateHypothesis> x, Node<StateHypothesis> y)
        {
            var a = (HypothesisTree)x;
            var b = (HypothesisTree)y;

            if (a.ChildProbability() < b.ChildProbability())
                return 1;
            return 0;
        }
        
        /// <summary>
        /// Remove lowest probability child nodes until k nodes remain
        /// </summary>
        /// <param name="numRemaining">Final number of nodes after pruning</param>
        public void Prune(int numRemaining)
        {
            if (Children.Count <= 0) return;
            Children.Sort(ProbCompare);

            while (Children.Count > numRemaining)
                Children.RemoveAt(Children.Count - 1);
        }

        public double ChildProbability()
        {
            var prob = TraverseChildProbabilities(this);
            return prob;
        }

        private double TraverseChildProbabilities(Node<StateHypothesis> inputNode)
        {
            var prob = double.MaxValue;
            if (inputNode.Children.Count > 0)
                prob = inputNode.Children.Select(TraverseChildProbabilities).Concat(new[] {prob}).Min();
            else
                return NodeData.Probability;

            return prob + NodeData.Probability;
        }

        public HypothesisTree GetChild(int index)
        {
            var child = (HypothesisTree) Children[index];
            child.Q = Q;
            child.R = R;
            child.P = P;
            child.H = H;
            child.F = F;
            
            foreach (var thisChild in Children)
            thisChild.Parent = null;

            Children = null;
            
            return child;
        }
    }

    public struct Vehicle
    {
        public List<StateEstimate> StateHistory;

        public Vehicle(StateEstimate initialState)
        {
            StateHistory = new List<StateEstimate> {initialState};
        }

        public Vehicle(IEnumerable<StateEstimate> stateHistoryOld, StateEstimate currentState)
        {
            StateHistory = new List<StateEstimate>(stateHistoryOld) {currentState};
        }
    }




}

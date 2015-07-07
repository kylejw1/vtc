using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

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
            traverseLeafnodes = (x) => {
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
            traverse = (x) =>
            {
                numChildren++;
                if (x.Children == null || x.Children.Count == 0) //Not sure why checking against null doesn't work here, maybe IsNull is not implemented. Why isn't this problem encountered in other places?
                    return;
                else
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
            else
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
            double prob = TraverseChildProbabilities(this);
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

        //public void SaveDeleted(string file_path, int current_frame, int min_path)
        //{
        //    object misValue = System.Reflection.Missing.Value;
        //    int num_states = 13;
        //    if (nodeData.deleted_vehicles.Count > 0)
        //    {
        //        Excel.Application xlApp = new Excel.Application();
        //        xlApp.Application.DisplayAlerts = false;
        //        Excel.Workbook xlWorkBook;

        //        if (File.Exists(file_path))
        //        {
        //            if (File.GetAttributes(file_path).HasFlag(FileAttributes.ReadOnly))
        //                throw new System.Exception("Read only");

        //            xlWorkBook = xlApp.Workbooks.Open(file_path, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        //        }
        //        else
        //        {
        //            xlWorkBook = xlApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
        //        }


        //        string currentSheet = "Sheet1";
        //        Excel.Sheets excelSheets = xlWorkBook.Worksheets;
        //        Excel.Worksheet xlWorkSheet = (Excel.Worksheet)excelSheets.get_Item(currentSheet);

        //        foreach (Vehicle deletedVehicle in nodeData.deleted_vehicles)
        //        {
        //            if (deletedVehicle.state_history[deletedVehicle.state_history.Count - 1].path_length >= min_path && deletedVehicle.state_history[deletedVehicle.state_history.Count-1].turn != 3)
        //            {//Find the lowest unoccupied column during existance of this vehicle
        //                int current_index = 0;
        //                int x_column;
        //                int y_column;
        //                int vx_column;
        //                int vy_column;
        //                int covx_column;
        //                int covy_column;
        //                int cov_vx_column;
        //                int cov_vy_column;
        //                int is_pedestrian_column;
        //                int turn_column;
        //                int is_occupied_column;
        //                int path_length_column;
        //                int missed_detections_column;

        //                string col_string;
        //                string cell_string;

        //                bool column_located = false;
        //                while (!column_located)
        //                {//Check if columns for index current_index are free
        //                    bool index_is_free = true;
        //                    for (int i = 0; i < deletedVehicle.state_history.Count; i++)
        //                    {
        //                        is_occupied_column = (current_index) * num_states + 11;
        //                        col_string = GetExcelColumnName(is_occupied_column);
        //                        cell_string = col_string + (current_frame - i).ToString();
        //                        Excel.Range occupationCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                        if (occupationCell.Value == 1)
        //                            index_is_free = false;
        //                        Marshal.ReleaseComObject(occupationCell);
        //                        occupationCell = null;
        //                    }

        //                    if (index_is_free)
        //                        column_located = true;
        //                    else
        //                        current_index++;
        //                }

        //                x_column = (current_index) * num_states + 1;
        //                y_column = (current_index) * num_states + 2;
        //                vx_column = (current_index) * num_states + 3;
        //                vy_column = (current_index) * num_states + 4;
        //                covx_column = (current_index) * num_states + 5;
        //                covy_column = (current_index) * num_states + 6;
        //                cov_vx_column = (current_index) * num_states + 7;
        //                cov_vy_column = (current_index) * num_states + 8;
        //                is_pedestrian_column = (current_index) * num_states + 9;
        //                turn_column = (current_index) * num_states + 10;
        //                is_occupied_column = (current_index) * num_states + 11;
        //                path_length_column = (current_index) * num_states + 12;
        //                missed_detections_column = (current_index) * num_states + 13;

        //                //Now current_index contains a free index
        //                for (int i = 0; i < deletedVehicle.state_history.Count; i++)
        //                {
        //                    col_string = GetExcelColumnName(x_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range xCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    xCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].coordinates.x;
        //                    Marshal.ReleaseComObject(xCell);
        //                    xCell = null;

        //                    col_string = GetExcelColumnName(y_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range yCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    yCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].coordinates.y;
        //                    Marshal.ReleaseComObject(yCell);
        //                    yCell = null;

        //                    col_string = GetExcelColumnName(vx_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range vxCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    vxCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].vx;
        //                    Marshal.ReleaseComObject(vxCell);
        //                    vxCell = null;

        //                    col_string = GetExcelColumnName(vy_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range vyCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    vyCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].vy;
        //                    Marshal.ReleaseComObject(vyCell);
        //                    vyCell = null;

        //                    col_string = GetExcelColumnName(covx_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range covxCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    covxCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].cov_x;
        //                    Marshal.ReleaseComObject(covxCell);
        //                    covxCell = null;

        //                    col_string = GetExcelColumnName(covy_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range covyCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    covyCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].cov_y;
        //                    Marshal.ReleaseComObject(covyCell);
        //                    covyCell = null;

        //                    col_string = GetExcelColumnName(cov_vx_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range cov_vxCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    cov_vxCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].cov_vx;
        //                    Marshal.ReleaseComObject(cov_vxCell);
        //                    cov_vxCell = null;

        //                    col_string = GetExcelColumnName(cov_vy_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range cov_vyCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    cov_vyCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].cov_vy;
        //                    Marshal.ReleaseComObject(cov_vyCell);
        //                    cov_vyCell = null;

        //                    col_string = GetExcelColumnName(is_pedestrian_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range is_pedestrianCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    is_pedestrianCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].is_pedestrian;
        //                    Marshal.ReleaseComObject(is_pedestrianCell);
        //                    is_pedestrianCell = null;

        //                    col_string = GetExcelColumnName(path_length_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range path_lengthCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    path_lengthCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].path_length;
        //                    Marshal.ReleaseComObject(path_lengthCell);
        //                    path_lengthCell = null;

        //                    col_string = GetExcelColumnName(missed_detections_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range missed_detectionsCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    missed_detectionsCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - i - 1].missed_detections;
        //                    Marshal.ReleaseComObject(missed_detectionsCell);
        //                    missed_detectionsCell = null;

        //                    col_string = GetExcelColumnName(turn_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range turnCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    turnCell.Value = deletedVehicle.state_history[deletedVehicle.state_history.Count - 1].turn;
        //                    Marshal.ReleaseComObject(turnCell);
        //                    turnCell = null;

        //                    col_string = GetExcelColumnName(is_occupied_column);
        //                    cell_string = col_string + (current_frame - i).ToString();
        //                    Excel.Range occupationCell = (Excel.Range)xlWorkSheet.get_Range(cell_string, cell_string);
        //                    occupationCell.Value = 1;
        //                    Marshal.ReleaseComObject(occupationCell);
        //                    occupationCell = null;

        //                }

        //            }
        //        }

        //        xlWorkBook.Close(true, file_path, false);
        //        xlApp.Quit();
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();

        //        Marshal.FinalReleaseComObject(xlWorkSheet);
        //        Marshal.FinalReleaseComObject(excelSheets);
        //        Marshal.FinalReleaseComObject(xlWorkBook);
        //        //xlApp.Application.Quit();
        //        //xlApp.Quit();
        //        Marshal.FinalReleaseComObject(xlApp);
        //        xlWorkSheet = null;
        //        excelSheets = null;
        //        xlWorkBook = null;
        //        xlApp = null;
        //    }
        //}

        private string GetExcelColumnName(int columnNumber)
        {
            var dividend = columnNumber;
            var columnName = string.Empty;

            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }

    /// <summary>
    /// Holds 2D position and velocity estimates for Kalman filtering
    /// </summary>
    public struct StateEstimate
    {
        //public Measurements measurements;
        public double X;
        public double Y;
        public double CovX;          //Location covariances
        public double CovY;

        public double Vx;             //Velocity estimates
        public double Vy;
        public double CovVx;         //Velocity covariances
        public double CovVy;

        public double Red;
        public double Green;
        public double Blue;

        public double CovRed;
        public double CovGreen;
        public double CovBlue;

        public double PathLength;    //Total path length travelled so far
        public Turn Turn;              //enum Turn to indicate turn decision (left, right or straight)
        
        public bool IsPedestrian;    //Binary flag set if object is likely to be a pedestrian

        public int MissedDetections; //Total number of times this object has not been detected during its lifetime

        public StateEstimate PropagateStateNoMeasurement(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, double compensationGain)
        {
            var updatedState = new StateEstimate
            {
                Turn = Turn,
                IsPedestrian = IsPedestrian,
                MissedDetections = MissedDetections + 1,
                PathLength =
                    PathLength + Math.Sqrt(Math.Pow((timestep*Vx), 2) + Math.Pow((timestep*Vy), 2))
            };

            var zEst = new DenseMatrix(7, 1); //4-Row state vector: x, vx, y, vy
            zEst[0, 0] = X;
            zEst[1, 0] = Vx;
            zEst[2, 0] = Y;
            zEst[3, 0] = Vy;
            zEst[4, 0] = Red;
            zEst[5, 0] = Green;
            zEst[6, 0] = Blue;

            var pBar = new DenseMatrix(7, 7);
            pBar[0, 0] = CovX;
            pBar[1, 1] = CovVx;
            pBar[2, 2] = CovY;
            pBar[3, 3] = CovVy;
            pBar[4, 4] = CovRed;
            pBar[5, 5] = CovGreen;
            pBar[6, 6] = CovBlue;

            //DenseMatrix B = H * P_bar * H;
            var zNext = F * zEst;
            var fTranspose =(DenseMatrix) F.Transpose();
            var pNext = (F * pBar * fTranspose) + compensationGain * Q;

            //Move values from matrix form into object properties
            updatedState.X = zNext[0, 0];
            updatedState.Y = zNext[2, 0];
            updatedState.Vx = zNext[1, 0];
            updatedState.Vy = zNext[3, 0];
            updatedState.Red = zNext[4, 0];
            updatedState.Green = zNext[5, 0];
            updatedState.Blue = zNext[6, 0];

            updatedState.CovX  = pNext[0, 0];
            updatedState.CovVx = pNext[1, 1];
            updatedState.CovY  = pNext[2, 2];
            updatedState.CovVy = pNext[3, 3];
            updatedState.CovRed = pNext[4, 4];
            updatedState.CovGreen = pNext[5, 5];
            updatedState.CovBlue = pNext[6, 6];

            return updatedState;
        }

        public StateEstimate PropagateState(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, Measurements measurements)
        {
            var updatedState = new StateEstimate
            {
                Turn = Turn,
                PathLength =
                    PathLength + Math.Sqrt(Math.Pow((timestep*Vx), 2) + Math.Pow((timestep*Vy), 2))
            };

            var zEst = new DenseMatrix(7, 1); //7-Row state vector: x, vx, y, vy, r, g, b
            zEst[0, 0] = X;
            zEst[1, 0] = Vx;
            zEst[2, 0] = Y;
            zEst[3, 0] = Vy;
            zEst[4, 0] = Red;
            zEst[5, 0] = Green;
            zEst[6, 0] = Blue;

            var zMeas = new DenseMatrix(5, 1); //5-Row measurement vector: x,y,r,g,b
            zMeas[0, 0] = measurements.X;
            zMeas[1, 0] = measurements.Y;
            zMeas[2, 0] = measurements.Red;
            zMeas[3, 0] = measurements.Green;
            zMeas[4, 0] = measurements.Blue;

            var pBar = new DenseMatrix(7, 7);
            pBar[0, 0] = CovX;
            pBar[1, 1] = CovVx;
            pBar[2, 2] = CovY;
            pBar[3, 3] = CovVy;
            pBar[4, 4] = CovRed;
            pBar[5, 5] = CovGreen;
            pBar[6, 6] = CovBlue;

            //DenseMatrix B = H * P_bar * H;
            var zNext = F * zEst;
            var fTranspose = (DenseMatrix)F.Transpose();
            var pNext = (F * pBar * fTranspose) + Q;
            var yResidual = zMeas - H * zNext;
            var hTranspose = (DenseMatrix)H.Transpose();
            var s = H * pNext * hTranspose + R;
            var sInv =(DenseMatrix) s.Inverse();
            var k = pNext * hTranspose *sInv;
            var zPost = zNext + k * yResidual;
            var pPost = (DenseMatrix.Identity(7) - k * H) * pNext;

            //Move values from matrix form into object properties
            updatedState.CovX = pPost[0, 0];
            updatedState.CovVx = pPost[1, 1];
            updatedState.CovY = pPost[2, 2];
            updatedState.CovVy = pPost[3, 3];
            updatedState.CovRed = pPost[4, 4];
            updatedState.CovGreen = pPost[5, 5];
            updatedState.CovBlue = pPost[6, 6];

            updatedState.X = zPost[0, 0];
            updatedState.Vx = zPost[1, 0];
            updatedState.Y = zPost[2, 0];
            updatedState.Vy = zPost[3, 0];
            updatedState.Red = zPost[4, 0];
            updatedState.Green = zPost[5, 0];
            updatedState.Blue = zPost[6, 0];

            return updatedState;
        }

        public static DenseMatrix Residual(DenseMatrix zEst, DenseMatrix zMeas)
        {
            var residual = zMeas - zEst;
            return residual;
        }

    }

    public struct Measurements
    {
        public double X;
        public double Y;
        public double Red;
        public double Green;
        public double Blue;
    }

    public enum Turn { Left, Right, Straight, Unknown};

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

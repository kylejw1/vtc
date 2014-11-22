using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
//using Excel = Microsoft.Office.Interop.Excel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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
namespace TreeLib
{

    public enum MeasurementSource { false_positive, existing_vehicle, new_vehicle }; 

    public class Node<T>
    {
        public Node<T> parent;
        public List<Node<T>> children;

        public T nodeData; 

        public Node(T value)
        {
            nodeData = value;
            children = new List<Node<T>>();
        }

        public Node()
        {
            children = new List<Node<T>>();
        }

        public Node<T> GetRoot()
        {            
            
            Func<Node<T>, Node<T>> getParent = null;
            getParent = (x) =>
            {
                if (x.parent != null) { return getParent(x.parent); } else { return this; }
            };

            return getParent(this);
        }

        public virtual void AddChild(T value)
        {
            Node<T> newchild = new Node<T>(value);
            newchild.parent = this;
            if (newchild != null && children != null)
                this.children.Add(newchild);
        }

        //Recursive function used in GetChain to iterate upwards through tree pushing each new node to the front of a list 
        private List<Node<T>> AddBack(List<Node<T>> x)
        {
        if (x[0].parent != null)
                {
                    x.Add(x[0].parent); //If parent node exists, add to start of the SortedList 
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
            List<Node<T>> pathList = new List<Node<T>>();
            pathList.Add(this);
            return AddBack(pathList);
        }

        /// <summary>
        /// Return list containing all nodes in this tree
        /// </summary>
        public List<Node<T>> ToList()
        {
            List<Node<T>> list = new List<Node<T>>();
            Node<T> root = this.GetRoot();
            Action<Node<T>> traverse = null;
            traverse = (x) => { list.Add(x); x.children.ForEach(traverse); };
            traverse(root);
            return list;
        }

        /// <summary>
        /// Return list containing all leaf nodes in this tree
        /// </summary>
        public List<Node<T>> GetLeafNodes()
        {
            List<Node<T>> list = new List<Node<T>>();
            Node<T> root = this.GetRoot();
            Action<Node<T>> traverse_leafnodes = null;
            traverse_leafnodes = (x) => {
               if (x.children == null || x.children.Count == 0) //Not sure why checking against null doesn't work here, maybe IsNull is not implemented. Why isn't this problem encountered in other places?
                   list.Add(x); 
               else
                x.children.ForEach(traverse_leafnodes); 
            };
            traverse_leafnodes(root);
            return list;
        }

        //Get number of nodes from root to leaf nodes, following "first child" path. This assumes that all branches are of equal depth. 
        public int TreeDepth()
        {
            int depth = TraverseDepth(this);
            return depth;
        }

        private int TraverseDepth(Node<T> inputNode)
        { int depth = 0;
        if (inputNode.children.Count > 0)
            foreach (Node<T> thisChild in inputNode.children)
            {
                int thisChildDepth = TraverseDepth(thisChild);
                if (thisChildDepth > depth)
                    depth = thisChildDepth;

            }

            return depth+1;
        }

        //Get this node's depth
        public int NodeDepth()
        {
            int node_depth = 1;
            Node<T> current_node = this;
                    while (current_node.parent != null)
                {
                    current_node = current_node.parent;
                    node_depth++;
                }
            return node_depth;
        }

        //Return array populated with # of nodes at each depth 
        public int[] NodeCountByDepth()
        {
            int tree_depth = this.TreeDepth();
            int[] depth_array = new int[tree_depth];
            List<Node<T>> node_list = this.ToList();
            foreach (Node<T> this_node in node_list)
                depth_array[this_node.NodeDepth() - 1]++;

            return depth_array;
        }

        public int numChildren()
        {
            int num_children = 0;
            Action<Node<T>> traverse = null;
            traverse = (x) =>
            {
                num_children++;
                if (x.children == null || x.children.Count == 0) //Not sure why checking against null doesn't work here, maybe IsNull is not implemented. Why isn't this problem encountered in other places?
                    return;
                else
                    x.children.ForEach(traverse);
            };
            traverse(this);
            return num_children-1;
        }

    }


    /// <summary>
    /// Object containing data-measurement association, associate state estimates and all other associated probablities and esimates
    /// necessary for multiple hypothesis tracking. 
    /// </summary>
    public class StateHypothesis
    {
        public double probability;

        public bool[,] assignment_matrix;

        public List<Vehicle> deleted_vehicles;

        public List<Vehicle> vehicles;

        public List<Vehicle> new_vehicles;

        public int miss_detection_threshold;

        public StateHypothesis(int miss_threshold)
        {
            probability = 1;
            assignment_matrix = new bool[0,0];
            vehicles = new List<Vehicle>();
            deleted_vehicles = new List<Vehicle>();
            new_vehicles = new List<Vehicle>();
            miss_detection_threshold = miss_threshold;
        }

        public StateHypothesis(double initial_probablity, int num_vehicles, int num_measurements,int miss_threshold)
        {
            probability = initial_probablity;
            assignment_matrix = new bool[num_measurements, num_vehicles + 2];
            deleted_vehicles = new List<Vehicle>();
            vehicles = new List<Vehicle>(num_vehicles);
            miss_detection_threshold = miss_threshold;
        }

        public StateEstimate[] GetStateEstimates()
        {
            StateEstimate[] vehicleStateEstimates = new StateEstimate[vehicles.Count];
            for (int i = 0; i < vehicles.Count; i++)
                vehicleStateEstimates[i] = vehicles[i].state_history[vehicles[i].state_history.Count - 1];
            
            return vehicleStateEstimates;
        }

        public StateEstimate[,] GetDeletedStateEstimates()
        {
            StateEstimate[,] deletedVehicleStateEstimates = new StateEstimate[deleted_vehicles.Count,2];
            for (int i = 0; i < deleted_vehicles.Count; i++)
            {
                try
                {
                    deletedVehicleStateEstimates[i, 0] = deleted_vehicles[i].state_history[0];
                }
                catch (Exception e)
                {
                    throw new System.ArgumentException("Initial state history missing");
                }

                try
                {
                    deletedVehicleStateEstimates[i, 1] = deleted_vehicles[i].state_history[deleted_vehicles[i].state_history.Count - 1];
                }
                catch (Exception e)
                {
                    string error_string = "Final state history missing. History length: " + deleted_vehicles[i].state_history.Count.ToString();
                    throw new System.ArgumentException(error_string);
                }
            }

            return deletedVehicleStateEstimates;
        }

        public StateEstimate[] GetNewStateEstimates()
        {
            StateEstimate[] newVehicleStateEstimates = new StateEstimate[new_vehicles.Count];
            for (int i = 0; i < new_vehicles.Count; i++)
                newVehicleStateEstimates[i] = new_vehicles[i].state_history[new_vehicles[i].state_history.Count-1];

            return newVehicleStateEstimates;
        }

        public void AddVehicle(int x, int y, int vx, int vy)
        {
            StateEstimate initial_state = new StateEstimate();
            Coordinates vehicleLocation = new Coordinates();
            vehicleLocation.x = x;
            vehicleLocation.y = y;
            initial_state.coordinates = vehicleLocation;
            initial_state.vx = vx;
            initial_state.vy = vy;
            initial_state.is_pedestrian = false;
            Vehicle newVehicle = new Vehicle(initial_state);
            vehicles.Add(newVehicle);
            new_vehicles.Add(newVehicle);
        }

        public void AddPedestrian(int x, int y,int vx, int vy)
        {
            StateEstimate initial_state = new StateEstimate();
            Coordinates vehicleLocation = new Coordinates();
            vehicleLocation.x = x;
            vehicleLocation.y = y;
            initial_state.coordinates = vehicleLocation;
            initial_state.vx = vx;
            initial_state.vy = vy;
            initial_state.is_pedestrian = true;
            Vehicle newVehicle = new Vehicle(initial_state);
            vehicles.Add(newVehicle);
            new_vehicles.Add(newVehicle);
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

        public double compensation_gain = 30; //Gain applied to process noise when a measurement is missed

        public double q = 10.0; // Process noise matrix multiplier
        public double r = 10.0; // Measurement noise matrix multiplier

        public double dt = 0.033; //Timestep between frames
        //public double dt = 1.00; //Timestep between frames

        public HypothesisTree(StateHypothesis value) : base(value)
        {
            H = new DenseMatrix(2, 4); // Measurement equation: only x and y are directly observed
            H[0, 0] = 1;
            H[1, 2] = 1;

            //  x_new  = x_old + dt*vx;
            //  vy_new = vy_old
            //  y_new  = y_old + dt*vy
            //  vx_new = vx_old
            F = new DenseMatrix(4, 4); //Motion equation
            F[0, 0] = 1;
            F[0, 1] = dt;
            F[1, 1] = 1;
            F[2, 2] = 1;
            F[2, 3] = dt;
            F[3, 3] = 1;

            Q = new DenseMatrix(4, 4); //Process covariance
            Q[0, 0] = (dt * dt * dt *dt / 4) * q;
            Q[0, 1] = (dt * dt * dt / 3) * q;
            Q[1, 0] = (dt * dt * dt / 3) * q;
            Q[1, 1] = (dt * dt / 2) * q;
            Q[2, 2] = (dt * dt * dt * dt/ 4) * q;
            Q[2, 3] = (dt * dt * dt / 3) * q;
            Q[3, 2] = (dt * dt * dt / 3) * q;
            Q[3, 3] = (dt * dt / 2) * q;

            //Previously used covariances (probably incorrect after reviewing a Wikipedia example of Kalman filter implementation)
            //Q[0, 0] = (dt * dt * dt / 3)*q;
            //Q[0, 1] = (dt * dt / 2)*q;
            //Q[1, 0] = (dt * dt / 2)*q;
            //Q[1, 1] = (dt)*q;
            //Q[2, 2] = (dt * dt * dt / 3)*q;
            //Q[2, 3] = (dt * dt / 2)*q;
            //Q[3, 2] = (dt * dt / 2)*q;
            //Q[3, 3] = (dt) * q;
                        

            R = new DenseMatrix(2, 2); //Measurement covariance
            R[0, 0] = r;
            R[1, 1] = r;

        }

        /// <summary>
        /// Create new child hypothesis
        /// </summary>
        /// <param name="value">New child's StateHypothesis</param>
        public override void AddChild(StateHypothesis value)
        {
            HypothesisTree newchild = new HypothesisTree(value);
            newchild.parent = this;
            if (newchild != null && children != null)
                this.children.Add(newchild);
        }

        /// <summary>
        /// Copies & updates state history of a vehicle from parent node referenced by integer
        /// </summary>
        /// <param name="address">Index of vehicle to be updated in parent StateHypothesis</param>
        /// <param name="current_state">New state of vehicle to be updated</param>
        public void UpdateVehicleFromPrevious(int address, StateEstimate current_state, bool with_measurement)
        {
            StateHypothesis parent_hypothesis = this.parent.nodeData;
            Vehicle last_frame_vehicle = parent_hypothesis.vehicles[address];
            current_state.is_pedestrian = last_frame_vehicle.state_history[last_frame_vehicle.state_history.Count - 1].is_pedestrian;
            Vehicle updated_vehicle = new Vehicle(last_frame_vehicle.state_history, current_state);
            if (!with_measurement)
                current_state.missed_detections++;
            else
                current_state.missed_detections = 0; 

            if (current_state.missed_detections < this.nodeData.miss_detection_threshold)
                nodeData.vehicles.Add(updated_vehicle);
            else
                nodeData.deleted_vehicles.Add(updated_vehicle);
        }


        /// <summary>
        /// Compare probability of two hypotheses
        /// </summary>
        /// <param name="x">Reference to first node</param>
        /// <param name="y">Refernce to second node</param>
        /// <returns>1 if p(a)>p(b), 0 otherwise</returns>
        private static int ProbCompare(Node<StateHypothesis> x, Node<StateHypothesis> y)
        {
            HypothesisTree a = (HypothesisTree)x;
            HypothesisTree b = (HypothesisTree)y;

            if (a.ChildProbability() < b.ChildProbability())
                return 1;
            else
                return 0;
        }
        
        /// <summary>
        /// Remove lowest probability child nodes until k nodes remain
        /// </summary>
        /// <param name="num_remaining">Final number of nodes after pruning</param>
        public void Prune(int num_remaining)
        {
            if (children.Count > 0)
            {
                this.children.Sort(ProbCompare);

                while (this.children.Count > num_remaining)
                    this.children.RemoveAt(children.Count - 1);
            }
        
        }

        public double ChildProbability()
        {
            double prob = TraverseChildProbabilities(this);
            return prob;
        }

        private double TraverseChildProbabilities(Node<StateHypothesis> inputNode)
        {
            double prob = double.MaxValue;
            if (inputNode.children.Count > 0)
                foreach (Node<StateHypothesis> thisChild in inputNode.children)
                {
                    double thisChildDepth = TraverseChildProbabilities(thisChild);
                    if (thisChildDepth < prob)
                        prob = thisChildDepth;
                }
            else
                return this.nodeData.probability;

            return prob + this.nodeData.probability;
        }

        public HypothesisTree GetChild(int index)
        {
            HypothesisTree child = (HypothesisTree) this.children[index];
            
            foreach (Node<StateHypothesis> thisChild in children)
            thisChild.parent = null;

            this.children = null;
            
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
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    }

    /// <summary>
    /// Holds 2D position and velocity estimates for Kalman filtering
    /// </summary>
    public struct StateEstimate
    {
        public Coordinates coordinates;
        public double cov_x;          //Location covariances
        public double cov_y;

        public double vx;             //Velocity estimates
        public double vy;
        public double cov_vx;         //Velocity covariances
        public double cov_vy;

        public double path_length;    //Total path length travelled so far
        public int turn;              //enum Turn to indicate turn decision (left, right or straight)
        
        public bool is_pedestrian;    //Binary flag set if object is likely to be a pedestrian

        public int missed_detections; //Total number of times this object has not been detected during its lifetime

        public StateEstimate PropagateStateNoMeasurement(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, double compensation_gain)
        {
            StateEstimate updatedState = new StateEstimate();
            //updatedState.coordinates.x = this.coordinates.x + timestep * this.vx;
            //updatedState.coordinates.y = this.coordinates.y + timestep * this.vy;
            updatedState.turn = this.turn;
            updatedState.is_pedestrian = this.is_pedestrian;
            updatedState.missed_detections = this.missed_detections + 1;
            updatedState.path_length = this.path_length + Math.Sqrt(Math.Pow((timestep * this.vx), 2) + Math.Pow((timestep * this.vy), 2));

            DenseMatrix z_est = new DenseMatrix(4, 1); //4-Row state vector: x, vx, y, vy
            z_est[0, 0] = this.coordinates.x;
            z_est[1, 0] = this.vx;
            z_est[2, 0] = this.coordinates.y;
            z_est[3, 0] = this.vy;

            DenseMatrix P_bar = new DenseMatrix(4, 4);
            P_bar[0, 0] = this.cov_x;
            P_bar[1, 1] = this.cov_vx;
            P_bar[2, 2] = this.cov_y;
            P_bar[3, 3] = this.cov_vy;

            //DenseMatrix B = H * P_bar * H;
            DenseMatrix z_next = F * z_est;
            DenseMatrix F_transpose =(DenseMatrix) F.Transpose();
            DenseMatrix P_next = (F * P_bar * F_transpose) + compensation_gain * Q;

            //Move values from matrix form into object properties
            updatedState.cov_x  = P_next[0, 0];
            updatedState.cov_vx = P_next[1, 1];
            updatedState.cov_y  = P_next[2, 2];
            updatedState.cov_vy = P_next[3, 3];

            updatedState.coordinates.x = z_next[0, 0];
            updatedState.coordinates.y = z_next[2, 0];
            updatedState.vx = z_next[1, 0];
            updatedState.vy = z_next[3, 0];

            return updatedState;
        }

        public StateEstimate PropagateState(double timestep, DenseMatrix H, DenseMatrix R, DenseMatrix F, DenseMatrix Q, Coordinates measurement)
        {
            StateEstimate updatedState = new StateEstimate();
            //updatedState.coordinates.x = this.coordinates.x + timestep * this.vx;
            //updatedState.coordinates.y = this.coordinates.y + timestep * this.vy;
            updatedState.turn = this.turn;
            updatedState.path_length = this.path_length + Math.Sqrt(Math.Pow((timestep * this.vx), 2) + Math.Pow((timestep * this.vy), 2));

            DenseMatrix z_est = new DenseMatrix(4, 1); //4-Row state vector: x, vx, y, vy
            z_est[0, 0] = this.coordinates.x;
            z_est[1, 0] = this.vx;
            z_est[2, 0] = this.coordinates.y;
            z_est[3, 0] = this.vy;

            DenseMatrix z_meas = new DenseMatrix(2, 1); //2-Row state vector: x, y
            z_meas[0, 0] = measurement.x;
            z_meas[1, 0] = measurement.y;


            DenseMatrix P_bar = new DenseMatrix(4, 4);
            P_bar[0, 0] = this.cov_x;
            P_bar[1, 1] = this.cov_vx;
            P_bar[2, 2] = this.cov_y;
            P_bar[3, 3] = this.cov_vy;

            //DenseMatrix B = H * P_bar * H;
            DenseMatrix z_next = F * z_est;
            DenseMatrix F_transpose = (DenseMatrix)F.Transpose();
            DenseMatrix P_next = (F * P_bar * F_transpose) + Q;
            DenseMatrix y_residual = z_meas - H * z_next;
            DenseMatrix H_transpose = (DenseMatrix)H.Transpose();
            DenseMatrix S = H * P_next * H_transpose + R;
            DenseMatrix S_inv =(DenseMatrix) S.Inverse();
            DenseMatrix K = P_next * H_transpose *S_inv;
            DenseMatrix z_post = z_next + K * y_residual;
            DenseMatrix P_post = (DenseMatrix.Identity(4) - K * H) * P_next;

            //Move values from matrix form into object properties
            updatedState.cov_x = P_post[0, 0];
            updatedState.cov_vx = P_post[1, 1];
            updatedState.cov_y = P_post[2, 2];
            updatedState.cov_vy = P_post[3, 3];

            updatedState.coordinates.x = z_post[0, 0];
            updatedState.coordinates.y = z_post[2, 0];
            updatedState.vx = z_post[1, 0];
            updatedState.vy = z_post[3, 0];

            return updatedState;
        }

        public static DenseMatrix residual(DenseMatrix z_est, DenseMatrix z_meas)
        {
            DenseMatrix residual = z_meas - z_est;
            return residual;
        }

    }

    public struct Coordinates
    {
        public double x;
        public double y;
    }

    enum Turn { Left, Right, Straight, Unknown};

    public struct Vehicle
    {
        public const double covx_init = 2;
        public const double covy_init = 2;
        public const double vx_init = 0;
        public const double vy_init = 0;
        public const double cov_vx_init = 300;
        public const double cov_vy_init = 300;

        public List<StateEstimate> state_history;

        public Vehicle(StateEstimate initial_state)
        {
            initial_state.cov_vx = cov_vx_init;
            initial_state.cov_vy = cov_vy_init;
            initial_state.cov_x = covx_init;
            initial_state.cov_y = covy_init;
            initial_state.turn = (int)Turn.Unknown;
            state_history = new List<StateEstimate>();
            state_history.Add(initial_state);
        }

        public Vehicle(List<StateEstimate> state_history_old, StateEstimate current_state)
        {
            state_history = new List<StateEstimate>(state_history_old);
            state_history.Add(current_state);
        }
    }




}

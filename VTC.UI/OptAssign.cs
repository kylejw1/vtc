using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VTC
{

    public struct MurtyNode
    {
        //Each int[] element is an assignment pair - [ detection, assigment ]
        public List<int[]> inclusion_list; //This node ensures every assignment in the inclusion list
        public List<int[]> exclusion_list; //This node excludes every assignment in the exclusion list

        public MurtyNode(List<int[]> inclusions, List<int[]> exclusions)
        {
            this.inclusion_list = inclusions;
            this.exclusion_list = exclusions;
        }
    }

    public class OptAssign
    {
        //This class takes an expanded assignment cost matrix and returns the k-best assignments.
        public static List<int[]> FindKBestAssignments(int[,] costs, int k)
        {
            int num_targets = costs.GetLength(0);
            if (k > num_targets)
                k = num_targets;

            List<int[]> kBestAssignments = new List<int[]>();
            for (int i = 0; i < k; i++)
            {
                int[] this_assignment = BestAssignment(costs);
                kBestAssignments.Add(this_assignment);
                if (k > (i + 1))
                {
                List<MurtyNode> partition = partition_assignment(this_assignment);
                MurtyNode best_node = findBestNode(costs, partition);
                costs = adjusted_cost(costs, best_node);
                    }
            }

            return kBestAssignments;
        }

        public static List<int[]> FindKBestAssignments(double[,] costs, int k)
        {
            double[,] temp_costs = (double[,]) costs.Clone();
            int num_targets = temp_costs.GetLength(0);
            if (k > num_targets)
                k = num_targets;

            

            List<int[]> kBestAssignments = new List<int[]>();
            for (int i = 0; i < k; i++)
            {
                
                int[] this_assignment = BestAssignment(temp_costs);
                kBestAssignments.Add(this_assignment);
                if (k > (i + 1))
                {
                    List<MurtyNode> partition = partition_assignment(this_assignment);
                    MurtyNode best_node = findBestNode(temp_costs, partition);
                    temp_costs = adjusted_cost(temp_costs, best_node);
                }
            }

            return kBestAssignments;
        }

        public static int[] BestAssignment(int[,] costs)
        { 
            int num_targets = costs.GetLength(0);
            int[,] temp_costs = (int[,]) costs.Clone();
            int[] assignment =  HungarianAlgorithm.FindAssignments(temp_costs);
            return assignment;
        }

        public static int[] BestAssignment(double[,] costs)
        {
            //Console.WriteLine("Calculating best assignment for costs:");
            //for (int i = 0; i < costs.GetLength(0); i++)
            //    for (int j = 0; j < costs.GetLength(1); j++)
            //    {
            //        Console.Write("i,j:{0}", costs[i, j]);
            //        if (costs[i, j] == double.NegativeInfinity)
            //            Console.WriteLine("Error condition here");
            //    }


            int num_targets = costs.GetLength(0);
            double[,] temp_costs = (double[,])costs.Clone();
            int[] assignment = HungarianAlgorithm.FindAssignments(temp_costs);
            return assignment;
        }

        public static List<MurtyNode> partition_assignment(int[] assignment)
        {
            List<MurtyNode> partition = new List<MurtyNode>();
            for (int i = 0; i < assignment.Length - 1; i++)
            {
                //Form inclusion list: fix all assignments where j < i
                List<int[]> inclusion_list = new List<int[]>();
                for (int j = 0; j < i; j++)
                {
                    int[] inclusion = { j, assignment[j] };
                    inclusion_list.Add(inclusion);
                }

                //Form exclusion list: exclude assignment i
                List<int[]> exclusion_list = new List<int[]>();
                int[] exclusion = { i, assignment[i] };
                exclusion_list.Add(exclusion);

                MurtyNode node = new MurtyNode(inclusion_list, exclusion_list);
                partition.Add(node);
            }

            return partition;
        }

        public static int[,] adjusted_cost(int[,] cost, MurtyNode node)
        {
            int[,] adjusted_costs = cost;
            node.exclusion_list.ForEach(delegate(int[] exclusion)
            {
                adjusted_costs[exclusion[0], exclusion[1]] = int.MaxValue;
            });

            node.inclusion_list.ForEach(delegate(int[] inclusion)
            {
                adjusted_costs[inclusion[0], inclusion[1]] = int.MinValue;
            });

            return adjusted_costs;
        }

        public static double[,] adjusted_cost(double[,] cost, MurtyNode node)
        {
            double[,] adjusted_costs = cost;
            node.exclusion_list.ForEach(delegate(int[] exclusion)
            {
                adjusted_costs[exclusion[0], exclusion[1]] = double.MaxValue;
            });

            node.inclusion_list.ForEach(delegate(int[] inclusion)
            {
                adjusted_costs[inclusion[0], inclusion[1]] = double.MinValue;
            });

            return adjusted_costs;
        }

        public static MurtyNode findBestNode(int[,] costs, List<MurtyNode> partition)
        {
            MurtyNode optimal_node = new MurtyNode();
            int least_cost = int.MaxValue;

            partition.ForEach(delegate(MurtyNode node)
            {
                int[,] adjusted_costs = adjusted_cost(costs, node);
                int[,] updated_costs = new int[,]{};
                int[] this_node_best_assignment = BestAssignment(adjusted_costs);
                int this_assignment_cost = assignmentCost(adjusted_costs, this_node_best_assignment);
                if (this_assignment_cost < least_cost)
                {
                    least_cost = this_assignment_cost;
                    optimal_node = node;
                }
            });

            return optimal_node;
        }

        public static MurtyNode findBestNode(double[,] costs, List<MurtyNode> partition)
        {
            MurtyNode optimal_node = new MurtyNode();
            double least_cost = double.MaxValue;

            partition.ForEach(delegate(MurtyNode node)
            {
                double[,] adjusted_costs = adjusted_cost(costs, node);
                double[,] updated_costs = new double[,] { };
                int[] this_node_best_assignment = BestAssignment(adjusted_costs);
                double this_assignment_cost = assignmentCost(adjusted_costs, this_node_best_assignment);
                if (this_assignment_cost < least_cost)
                {
                    least_cost = this_assignment_cost;
                    optimal_node = node;
                }
            });

            return optimal_node;
        }

        public static int assignmentCost(int[,] cost, int[] assignment)
        {
            int assignment_cost = 0;
            for (int i = 0; i < cost.GetLength(0); i++)
                    assignment_cost += cost[i, assignment[i]];

            return assignment_cost;
        }

        public static double assignmentCost(double[,] cost, int[] assignment)
        {
            double assignment_cost = 0;
            for (int i = 0; i < cost.GetLength(0); i++)
                assignment_cost += cost[i, assignment[i]];

            return assignment_cost;
        }

    }
}

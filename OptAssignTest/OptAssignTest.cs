using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using VTC.Kernel;

namespace OptAssignTest
{
    
    
    /// <summary>
    ///This is a test class for OptAssignTest and is intended
    ///to contain all OptAssignTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OptAssignTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for BestAssignment
        ///</summary>
        [TestMethod()]
        public void BestAssignmentTest()
        {
            int[,] costs = {{2,0},{0,2}}; // TODO: Initialize to an appropriate value
            int[] expected = {1,0}; // TODO: Initialize to an appropriate value
            int[,] updated_costs = {};
            var actual = OptAssign.BestAssignment(costs);

            string actual_string = "";
            foreach(var item in actual)
                actual_string += item.ToString() + ", ";

            string expected_string = "";
            foreach (var item in expected)
                expected_string += item.ToString() + ", ";

            Console.WriteLine("Answer is {0}", actual_string);
            Console.WriteLine("Expected is {0}", expected_string);

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BestAssignment(float)
        ///</summary>
        [TestMethod()]
        public void BestAssignmentFloatTest()
        {
            double[,] costs = { { 2.0, 0.0 }, { 0.0, 2.0 } }; // TODO: Initialize to an appropriate value
            int[] expected = { 1, 0 }; // TODO: Initialize to an appropriate value
            int[,] updated_costs = { };
            var actual = OptAssign.BestAssignment(costs);

            string actual_string = "";
            foreach (var item in actual)
                actual_string += item.ToString() + ", ";

            string expected_string = "";
            foreach (var item in expected)
                expected_string += item.ToString() + ", ";

            Console.WriteLine("Answer is {0}", actual_string);
            Console.WriteLine("Expected is {0}", expected_string);

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BestAssignment using rectangular cost matrix
        ///</summary>
        [TestMethod()]
        public void BestAssignmentRectangularTest()
        {
            int[,] costs = { { 2, 0, 5}, { 0, 2, 5} }; 
            int[] expected = { 1, 0 };
            int[,] updated_costs = { };
            var actual = OptAssign.BestAssignment(costs);

            string actual_string = "";
            foreach (var item in actual)
                actual_string += item.ToString() + ", ";

            string expected_string = "";
            foreach (var item in expected)
                expected_string += item.ToString() + ", ";

            Console.WriteLine("Answer is {0}", actual_string);
            Console.WriteLine("Expected is {0}", expected_string);

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FindKBestAssignments
        ///</summary>
        [TestMethod()]
        public void FindKBestAssignmentsTest()
        {
            int[,] costs = { {5,0}, {1,2} }; 
            int[,] updated_costs = {};
            int k = 2; 
            List<int[]> expected = new List<int[]>(); 
            int[] assignment1 = {1, 0};
            int[] assignment2 = {0, 1};
            expected.Add(assignment1);
            expected.Add(assignment2);

            var actual = OptAssign.FindKBestAssignments(costs, k);


            Assert.AreEqual(expected.Count, actual.Count);
            if(expected.Count == actual.Count)
            {
                for(int i=0; i < expected.Count; i++)
                CollectionAssert.AreEqual( expected[i], actual[i]);
            }

        }

        /// <summary>
        ///A test for FindKBestAssignments
        ///</summary>
        [TestMethod()]
        public void FindKBestAssignmentsDoubleTest()
        {
            double[,] costs = { { 5.0, 0.0 }, { 1.0, 2.0 } };
            double[,] updated_costs = { };
            int k = 2;
            List<int[]> expected = new List<int[]>();
            int[] assignment1 = { 1, 0 };
            int[] assignment2 = { 0, 1 };
            expected.Add(assignment1);
            expected.Add(assignment2);

            var actual = OptAssign.FindKBestAssignments(costs, k);

            Assert.AreEqual(expected.Count, actual.Count);
            if (expected.Count == actual.Count)
            {
                for (int i = 0; i < expected.Count; i++)
                    CollectionAssert.AreEqual(expected[i], actual[i]);
            }

        }

        /// <summary>
        ///A test for FindKBestAssignments - measurement starvation
        ///</summary>
        [TestMethod()]
        public void ExceedKBestAssignmentsDoubleTest()
        {
            double[,] costs = { { 5.0, 0.0 }, { 1.0, 2.0 } };
            double[,] updated_costs = { };
            int k = 4;
            List<int[]> expected = new List<int[]>();
            int[] assignment1 = { 1, 0 };
            int[] assignment2 = { 0, 1 };
            expected.Add(assignment1);
            expected.Add(assignment2);

            var actual = OptAssign.FindKBestAssignments(costs, k);

            Assert.AreEqual(expected.Count, actual.Count);
            if (expected.Count == actual.Count)
            {
                for (int i = 0; i < expected.Count; i++)
                    CollectionAssert.AreEqual(expected[i], actual[i]);
            }

        }

        /// <summary>
        ///A bigger test for FindKBestAssignments
        ///</summary>
        [TestMethod()]
        public void Find3x3KBestAssignmentsTest()
        {
            int[,] costs = { { 3,1,5}, {0,0,2}, { 2,1,6 } };
            int[,] updated_costs = { };
            int k = 2;
            List<int[]> expected = new List<int[]>();
            int[] assignment1 = { 1, 2, 0 };
            int[] assignment2 = { 2, 0, 1 }; //Note that this is an ambiguous case - {0,2,1} also yields the same cost
            expected.Add(assignment1);
            expected.Add(assignment2);

            var actual = OptAssign.FindKBestAssignments(costs, k);

            Assert.AreEqual(expected.Count, actual.Count);
            if (expected.Count == actual.Count)
            {
                for (int i = 0; i < expected.Count; i++)
                    CollectionAssert.AreEqual(expected[i], actual[i]);
            }

        }

        /// <summary>
        ///A test for partition_assignment
        ///</summary>
        [TestMethod()]
        public void partition_assignmentTest()
        {
            int[] assignment = {1,0};

            List<int[]> exclusion_list = new List<int[]> {new[] {0, 1}};
            List<int[]> inclusion_list = new List<int[]>();
            MurtyNode node1 = new MurtyNode(inclusion_list, exclusion_list);

            List<MurtyNode> expected = new List<MurtyNode> {node1};

            var actual = OptAssign.partition_assignment(assignment);

            Assert.AreEqual(expected.Count, actual.Count);

            if(expected.Count == actual.Count)
            for(int i=0; i < expected.Count; i++)
                compare_node(expected[i], actual[i]);
        }

        public void compare_node(MurtyNode a, MurtyNode b)
        {
            Assert.AreEqual(a.exclusion_list.Count, b.exclusion_list.Count);
            Assert.AreEqual(a.inclusion_list.Count, b.inclusion_list.Count);

            if (a.exclusion_list.Count == b.exclusion_list.Count && a.inclusion_list.Count == b.inclusion_list.Count)
            {
                for (int i = 0; i < a.exclusion_list.Count; i++)
                    CollectionAssert.AreEqual(a.exclusion_list[i], b.exclusion_list[i]);

                for (int i = 0; i < a.inclusion_list.Count; i++)
                    CollectionAssert.AreEqual(a.inclusion_list[i], b.inclusion_list[i]);
            }
        }

        /// <summary>
        ///A test for adjusted_cost
        ///</summary>
        [TestMethod()]
        public void adjusted_costTest()
        {
            int[,] cost = {{1,0},{0,1}}; 
            MurtyNode node = new MurtyNode(); // TODO: Initialize to an appropriate value
            List<int[]> inclusion_list = new List<int[]> {new[] {1, 0}};
            List<int[]> exclusion_list = new List<int[]> { new[] { 0, 0 } };
            node.exclusion_list = exclusion_list;
            node.inclusion_list = inclusion_list;

            int[,] expected = {{int.MaxValue, 0},{int.MinValue, 1}}; // TODO: Initialize to an appropriate value
            var actual = OptAssign.adjusted_cost(cost, node);

            for (int i = 0; i < cost.GetLength(0); i++)
                for (int j = 0; j < cost.GetLength(1); j++)
                {
                    Console.WriteLine("At {0},{1}, actual is {2} and expected is {3}", i, j, actual[i, j], expected[i, j]);
                }

            for (int i = 0; i < cost.GetLength(0); i++)
                for (int j = 0; j < cost.GetLength(1); j++)
                {
                    Assert.AreEqual(expected[i, j], actual[i, j]);
                }
        }

        /// <summary>
        ///A test for assignmentCost
        ///</summary>
        [TestMethod()]
        public void assignmentCostTest()
        {
            int[,] cost = { {5,1}, {2,3} }; // TODO: Initialize to an appropriate value
            int[] assignment = { 0, 1}; // TODO: Initialize to an appropriate value
            int expected = 8; // TODO: Initialize to an appropriate value
            var actual = OptAssign.assignmentCost(cost, assignment);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for findBestNode
        ///</summary>
        [TestMethod()]
        public void findBestNodeTest()
        {
            int[,] costs = { {3,1,5}, {0,0,2}, {2,1,6}  };
            int[] optimal_assignment = OptAssign.BestAssignment(costs);
            List<MurtyNode> partition = OptAssign.partition_assignment(optimal_assignment); // TODO: Initialize to an appropriate value

            MurtyNode expected = new MurtyNode(); // TODO: Initialize to an appropriate value
            List<int[]> exclusions = new List<int[]> {new[] {0, 1}};
            expected.exclusion_list = exclusions;
            List<int[]> inclusions = new List<int[]>();
            expected.inclusion_list = inclusions;

            var actual = OptAssign.findBestNode(costs, partition);

            Assert.AreEqual(expected.exclusion_list.Count, actual.exclusion_list.Count);
            Assert.AreEqual(expected.inclusion_list.Count, actual.inclusion_list.Count);

            if (expected.exclusion_list.Count == actual.exclusion_list.Count && expected.inclusion_list.Count == actual.inclusion_list.Count)
            {
                for (int i = 0; i < expected.exclusion_list.Count; i++)
                    CollectionAssert.AreEqual(expected.exclusion_list[i], actual.exclusion_list[i]);

                for (int i = 0; i < expected.inclusion_list.Count; i++)
                    CollectionAssert.AreEqual(expected.inclusion_list[i], actual.inclusion_list[i]);
            }

        }
    }
}

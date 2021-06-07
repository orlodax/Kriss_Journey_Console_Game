using kriss.Classes;
using lybra;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            DataLayer.Init();
        }

        /// <summary>
        /// Check if all nodes/choices in chapter 6 climbing are accessible
        /// </summary>
        [Test]
        public void AreAllClimbingNodesAccessible()
        {
            Chapter c6 = DataLayer.Chapters[5];

            List<NodeBase> nodes = c6.Nodes.FindAll(n =>n.Id > 90 && n.Type == "Choice");

            List<NodeBase> accessibleChildren = new();
            List<int> failingIds = new();

            bool willPass = true;

            foreach (NodeBase n in nodes)
            {
                foreach (Choice c in n.Choices)
                {
                    NodeBase child = c6.Nodes.Find(n => n.Id == c.ChildId);
                    if (child == null)
                    {
                        willPass = false;
                        failingIds.Add(c.ChildId);
                    }
                    else
                        accessibleChildren.Add(child);
                }

                // a choice node should't have a direct childid
                if (n.ChildId > 0)
                    willPass = false;
            }
            Assert.True(willPass);
        }

        /// <summary>
        /// DFS Check if a path to the top of the climbing in chapter 6 really exists
        /// </summary>
        [Test]
        public void IsPathValid()
        {
            bool isValid = false;
            //start 512
            //end 10
            Chapter c6 = DataLayer.Chapters[5];
            Stack<NodeBase> stack = new();

            NodeBase startNode = c6.Nodes.Find(n => n.Id == 512);
            
            startNode.IsVisited = true;
            stack.Push(startNode);

            List<int> traversed = new();

            while (stack.Any())
            {
                NodeBase v = stack.Peek();
                traversed.Add(v.Id);
                stack.Pop();

                // exclude death node
                if (v.Id != 99)
                {
                    List<NodeBase> neighbors = new();

                    if (v.Choices != null && v.Choices.Any())
                        foreach (Choice c in v.Choices)
                            neighbors.Add(c6.Nodes.Find(n => n.Id == c.ChildId));

                    if (v.ChildId > 0)
                        neighbors.Add(c6.Nodes.Find(n => n.Id == v.ChildId));

                    foreach (NodeBase n in neighbors)
                    {
                        if (n.Id == 10)
                            isValid = true;

                        if (!n.IsVisited)
                        {
                            stack.Push(n);
                            n.IsVisited = true;
                        }
                    }
                }
            }
            Assert.True(isValid, "Traversed nodes: ", traversed);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests;

public class ClimbingSpireTests
{
    GameEngine gameEngine;


    [TestInitialize]
    public void TestInitialize()
    {
        gameEngine = GameEngineTestExtensions.Setup();
    }

    /// <summary>
    /// Check if all nodes/choices in chapter 6 climbing are accessible
    /// </summary>
    [TestMethod]
    public void AreAllClimbingNodesAccessible()
    {
        Chapter c6 = gameEngine.GetChapters()[5];

        List<NodeBase> accessibleChildren = [];
        List<int> failingIds = [];

        bool willPass = true;

        foreach (ChoiceNode n in c6.Nodes.OfType<ChoiceNode>())
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
        Assert.IsTrue(willPass);
    }

    /// <summary>
    /// DFS Check if a path to the top of the climbing in chapter 6 really exists
    /// </summary>
    [TestMethod]
    public void IsPathValid()
    {
        bool isValid = false;
        //start 512
        //end 10
        Chapter c6 = gameEngine.GetChapters()[5];
        Stack<NodeBase> stack = new();

        NodeBase startNode = c6.Nodes.Find(n => n.Id == 512);

        startNode.IsVisited = true;
        stack.Push(startNode);

        List<int> traversed = [];

        while (stack.Count != 0)
        {
            NodeBase v = stack.Peek();
            traversed.Add(v.Id);
            stack.Pop();

            // exclude death node
            if (v.Id == 99)
                continue;

            List<NodeBase> neighbors = [];

            if (v is ChoiceNode nc && nc.Choices != null && nc.Choices.Count != 0)
                foreach (Choice c in nc.Choices)
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
        Assert.IsTrue(isValid, "Traversed nodes: ", traversed);
    }
}

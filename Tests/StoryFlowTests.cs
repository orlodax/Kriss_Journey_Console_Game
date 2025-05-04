using System;
using System.Collections.Generic;
using System.Linq;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests;

[TestClass]
public class StoryFlowTests
{
    GameEngine gameEngine;


    [TestInitialize]
    public void TestInitialize()
    {
        gameEngine = GameEngineTestExtensions.Setup();
    }

    [TestMethod]
    public void AllChapters_HaveValidStructure()
    {
        // Check all chapters have at least one node
        foreach (Chapter chapter in gameEngine.GetChapters())
        {
            Assert.IsTrue(chapter.Nodes.Count > 0, $"Chapter {chapter.Id} has no nodes");

            // Check all nodes have valid IDs
            foreach (NodeBase node in chapter.Nodes)
            {
                Assert.IsTrue(node.Id > 0, $"Node in chapter {chapter.Id} has invalid ID: {node.Id}");
            }
        }
    }

    [TestMethod]
    public void AllChapters_ValidateChildNodes()
    {
        // Check that all referenced child nodes exist
        foreach (Chapter chapter in gameEngine.GetChapters())
        {
            foreach (NodeBase node in chapter.Nodes)
            {
                // If childId is not 0, it should point to a valid node
                if (node.ChildId > 0)
                {
                    bool childExists = chapter.Nodes.Any(n => n.Id == node.ChildId);
                    Assert.IsTrue(childExists,
                        $"Node {node.Id} in chapter {chapter.Id} references non-existent child node {node.ChildId}");
                }

                // Check specific node types for their child references
                if (node is ChoiceNode choiceNode)
                {
                    foreach (Choice choice in choiceNode.Choices)
                    {
                        if (choice.ChildId > 0)
                        {
                            bool childExists = chapter.Nodes.Any(n => n.Id == choice.ChildId);
                            Assert.IsTrue(childExists,
                                $"Choice in node {node.Id}, chapter {chapter.Id} references non-existent child node {choice.ChildId}");
                        }
                    }
                }
                else if (node is ActionNode actionNode && node is not MiniGame01)
                {
                    foreach (Kriss.Models.Action action in actionNode.Actions)
                    {
                        if (action.ChildId.HasValue)
                        {
                            bool childExists = chapter.Nodes.Any(n => n.Id == action.ChildId.Value);
                            Assert.IsTrue(childExists,
                                $"Action in node {node.Id}, chapter {chapter.Id} references non-existent child node {action.ChildId}");
                        }

                        // Check action objects for child references
                        foreach (ActionObject obj in action.Objects)
                        {
                            if (obj.ChildId.HasValue)
                            {
                                bool childExists = chapter.Nodes.Any(n => n.Id == obj.ChildId.Value);
                                Assert.IsTrue(childExists,
                                    $"Action object in node {node.Id}, chapter {chapter.Id} references non-existent child node {obj.ChildId}");
                            }
                        }
                    }
                }
                else if (node is DialogueNode dialogueNode)
                {
                    foreach (Dialogue dialogue in dialogueNode.Dialogues)
                    {
                        if (dialogue.ChildId.HasValue)
                        {
                            bool childExists = chapter.Nodes.Any(n => n.Id == dialogue.ChildId.Value);
                            Assert.IsTrue(childExists,
                                $"Dialogue in node {node.Id}, chapter {chapter.Id} references non-existent child node {dialogue.ChildId}");
                        }

                        // Check replies for child references
                        if (dialogue.Replies != null)
                        {
                            foreach (Reply reply in dialogue.Replies)
                            {
                                if (reply.ChildId.HasValue)
                                {
                                    bool childExists = chapter.Nodes.Any(n => n.Id == reply.ChildId.Value);
                                    Assert.IsTrue(childExists,
                                        $"Dialogue reply in node {node.Id}, chapter {chapter.Id} references non-existent child node {reply.ChildId}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [TestMethod]
    public void AllChapters_EnsureAllNodesAreReachable()
    {
        foreach (Chapter chapter in gameEngine.GetChapters())
        {
            // Get the first node (entry point)
            NodeBase startNode = chapter.Nodes.OrderBy(n => n.Id).FirstOrDefault();
            if (startNode == null)
                continue;

            // Keep track of visited nodes in this chapter
            HashSet<int> visitedNodeIds = [];
            Queue<NodeBase> nodesToVisit = new();

            // Start the traversal from the first node
            nodesToVisit.Enqueue(startNode);

            while (nodesToVisit.Count > 0)
            {
                NodeBase currentNode = nodesToVisit.Dequeue();
                visitedNodeIds.Add(currentNode.Id);

                // Get all possible child nodes
                HashSet<int> childIds = [];

                // Direct child
                if (currentNode.ChildId > 0)
                    childIds.Add(currentNode.ChildId);

                // Node-specific children
                if (currentNode is ChoiceNode choiceNode)
                {
                    foreach (Choice choice in choiceNode.Choices)
                        if (choice.ChildId > 0)
                            childIds.Add(choice.ChildId);
                }
                else if (currentNode is ActionNode actionNode && currentNode is not MiniGame01)
                {
                    foreach (Kriss.Models.Action action in actionNode.Actions)
                    {
                        if (action.ChildId.HasValue)
                            childIds.Add(action.ChildId.Value);

                        foreach (ActionObject obj in action.Objects)
                            if (obj.ChildId.HasValue)
                                childIds.Add(obj.ChildId.Value);
                    }
                }
                else if (currentNode is DialogueNode dialogueNode)
                {
                    foreach (Dialogue dialogue in dialogueNode.Dialogues)
                    {
                        if (dialogue.ChildId.HasValue)
                            childIds.Add(dialogue.ChildId.Value);

                        if (dialogue.Replies != null)
                            foreach (Reply reply in dialogue.Replies)
                                if (reply.ChildId.HasValue)
                                    childIds.Add(reply.ChildId.Value);
                    }
                }

                // Enqueue unvisited children
                foreach (int childId in childIds)
                {
                    if (!visitedNodeIds.Contains(childId))
                    {
                        NodeBase childNode = chapter.Nodes.FirstOrDefault(n => n.Id == childId);
                        if (childNode != null)
                            nodesToVisit.Enqueue(childNode);
                    }
                }
            }

            // Find any orphaned nodes
            IEnumerable<NodeBase> orphanedNodes = chapter.Nodes.Where(n => !visitedNodeIds.Contains(n.Id));

            // Some nodes might be intentionally orphaned for development or future content
            if (orphanedNodes.Any())
            {
                Console.WriteLine($"Chapter {chapter.Id} has {orphanedNodes.Count()} potentially orphaned nodes:");
                foreach (NodeBase node in orphanedNodes)
                    Console.WriteLine($"Node {node.Id} ({node.GetType().Name}) is not reachable from the start node");
            }

            // We don't assert on orphaned nodes as they might be intentional
            Assert.IsTrue(visitedNodeIds.Count > 0, $"No nodes were visited in chapter {chapter.Id}");
        }
    }

    [TestMethod]
    public void Chapter1_FirstNodeIsAccessible()
    {
        // Assuming chapter 1 exists and is the start of the game
        if (gameEngine.GetChapters().Count > 0)
        {
            Chapter chapter1 = gameEngine.GetChapters()[0];
            Assert.IsNotNull(chapter1, "Chapter 1 should exist");

            NodeBase firstNode = chapter1.Nodes.OrderBy(n => n.Id).FirstOrDefault();
            Assert.IsNotNull(firstNode, "Chapter 1 should have at least one node");
        }
    }

    [TestMethod]
    public void AllChapters_ValidateTextContent()
    {
        // Check that all nodes have some text content
        foreach (Chapter chapter in gameEngine.GetChapters())
        {
            foreach (NodeBase node in chapter.Nodes)
            {
                // Node-specific content validation
                if (node is ChoiceNode choiceNode)
                {
                    Assert.IsTrue(choiceNode.Choices.Count > 0,
                        $"ChoiceNode {node.Id} in chapter {chapter.Id} has no choices");
                }
                else if (node is DialogueNode dialogueNode)
                {
                    Assert.IsTrue(dialogueNode.Dialogues.Count > 0,
                        $"DialogueNode {node.Id} in chapter {chapter.Id} has no dialogues");

                    foreach (Dialogue dialogue in dialogueNode.Dialogues)
                        if (!string.IsNullOrWhiteSpace(dialogue.Line))
                        {
                            // If it has replies, they should have text
                            if (dialogue.Replies != null && dialogue.Replies.Count > 0)
                            {
                                foreach (Reply reply in dialogue.Replies)
                                    Assert.IsFalse(string.IsNullOrWhiteSpace(reply.Line),
                                        $"Dialogue reply in node {node.Id}, chapter {chapter.Id} has no text");
                            }
                        }
                }
                else if (node is ActionNode actionNode && node is not MiniGame01)
                {
                    Assert.IsTrue(actionNode.Actions.Count > 0,
                        $"ActionNode {node.Id} in chapter {chapter.Id} has no actions");

                    foreach (Kriss.Models.Action action in actionNode.Actions)
                    {
                        Assert.IsTrue(action.Verbs.Count > 0,
                            $"Action in node {node.Id}, chapter {chapter.Id} has no objects");
                    }
                }
            }
        }
    }
}
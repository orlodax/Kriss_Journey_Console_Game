using System;
using System.Collections.Generic;
using System.Linq;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;

namespace KrissJourney.Tests.Infrastructure.Helpers;

/// <summary>
/// Helpers for testing nodes without requiring actual chapters to be loaded
/// </summary>
public static class NodeTestHelper
{
    /// <summary>
    /// Create and configure a test node without requiring an actual chapter
    /// </summary>
    /// <typeparam name="T">The type of node to create</typeparam>
    /// <param name="gameEngine">The game engine instance</param>
    /// <param name="nodeId">The ID to assign to the node (defaults to 1)</param>
    /// <param name="configure">Optional action to configure the node</param>
    /// <returns>The configured node</returns>
    public static T CreateTestNode<T>(this GameEngine gameEngine, int nodeId = 1, Action<T> configure = null) where T : NodeBase, new()
    {
        var node = new T { Id = nodeId };
        // Apply default configuration
        if (typeof(T) == typeof(StoryNode) || node is StoryNode)
        {
            node.Text = "This is a test story node";
        }
        else if (typeof(T) == typeof(DialogueNode) || node is DialogueNode)
        {
            node.Text = "This is a test dialogue node";
            if (node is DialogueNode dialogueNode)
            {
                dialogueNode.Dialogues =
                [
                    new Dialogue { Actor = "Tester", Line = "Test dialogue" }
                ];
            }
        }
        else if (typeof(T) == typeof(ChoiceNode) || node is ChoiceNode)
        {
            node.Text = "This is a test choice node";
            if (node is ChoiceNode choiceNode)
            {
                choiceNode.Choices =
                [
                    new Choice { Desc = "Test choice", ChildId = 2 }
                ];
            }
        }
        else if (typeof(T) == typeof(ActionNode) || node is ActionNode)
        {
            node.Text = "This is a test action node";
            if (node is ActionNode actionNode)
            {
                actionNode.Actions =
                [
                    new Kriss.Models.Action { Verbs = ["Test action"] }
                ];
            }
        }

        // Apply custom configuration if provided
        configure?.Invoke(node);

        // Set up test environment for the node
        SetupTestEnvironmentForNode(gameEngine, node);

        return node;
    }/// <summary>
     /// Sets up the test environment for a node
     /// </summary>
     /// <param name="gameEngine">The game engine instance</param>
     /// <param name="node">The node to set up</param>
    private static void SetupTestEnvironmentForNode(GameEngine gameEngine, NodeBase node)
    {
        // Get fields using reflection
        var chaptersField = typeof(GameEngine).GetField("chapters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var chapters = chaptersField?.GetValue(gameEngine) as List<Chapter>;

        var currentChapterField = typeof(GameEngine).GetField("currentChapter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var currentNodeField = typeof(GameEngine).GetField("currentNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Create a test chapter if needed
        var testChapterId = 999;
        Chapter testChapter = chapters?.FirstOrDefault(c => c.Id == testChapterId);

        if (testChapter == null)
        {
            testChapter = new Chapter
            {
                Id = testChapterId,
                Title = "Test Chapter",
                Nodes = [node]
            };

            chapters?.Add(testChapter);
        }
        else if (!testChapter.Nodes.Any(n => n.Id == node.Id))
        {
            // Add the node to the test chapter if it's not already there
            testChapter.Nodes.Add(node);
        }

        // Set up the node
        node.SetGameEngine(gameEngine);

        // Modify our fields via reflection
        currentChapterField?.SetValue(gameEngine, testChapter);
        currentNodeField?.SetValue(gameEngine, node);

        // Patch the AdvanceToNext method for testing
        // We create a custom implementation for this node only
        var advanceToNextMethod = typeof(NodeBase).GetMethod("AdvanceToNext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (advanceToNextMethod != null)
        {
            // You can't directly replace the method, but you can modify the behavior 
            // by swapping out the GameEngine for a controlled one or using a mock
            // For now, we'll leave it as is
        }
    }

    /// <summary>
    /// Runs a test on a node without requiring an actual chapter
    /// </summary>
    /// <typeparam name="T">The type of node to test</typeparam>
    /// <param name="gameEngine">The game engine instance</param>
    /// <param name="testAction">The action to perform on the node</param>
    /// <param name="nodeId">The ID to assign to the node (defaults to 1)</param>
    /// <param name="configure">Optional action to configure the node</param>
    public static void TestNode<T>(this GameEngine gameEngine, Action<T> testAction, int nodeId = 1, Action<T> configure = null) where T : NodeBase, new()
    {
        var node = gameEngine.CreateTestNode(nodeId, configure);
        testAction(node);
    }
}

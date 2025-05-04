using System;
using System.IO;
using System.Text;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;

namespace KrissJourney.Tests;

public static class TestUtils
{
    internal static TextReader OriginalIn;
    internal static TextWriter OriginalOut;

    /// <summary>
    /// Sets up console input with predefined input strings
    /// </summary>
    public static void SetupConsoleInput(string[] inputs)
    {
        OriginalIn = Console.In;
        StringReader stringReader = new(string.Join(Environment.NewLine, inputs));
        Console.SetIn(stringReader);
    }

    /// <summary>
    /// Captures console output for testing
    /// </summary>
    public static StringBuilder SetupConsoleOutput()
    {
        OriginalOut = Console.Out;
        StringBuilder stringBuilder = new();
        StringWriter stringWriter = new(stringBuilder);
        Console.SetOut(stringWriter);
        return stringBuilder;
    }

    /// <summary>
    /// Restores original console input/output
    /// </summary>
    public static void RestoreConsole()
    {
        if (OriginalIn != null)
            Console.SetIn(OriginalIn);
        if (OriginalOut != null)
            Console.SetOut(OriginalOut);
    }

    /// <summary>
    /// Creates a simple mock ActionNode for testing
    /// </summary>
    public static ActionNode CreateMockActionNode()
    {
        return new ActionNode
        {
            Id = 1,
            Text = "Test Action Node",
            ChildId = 2,
            Actions =
            [
                new() {
                    Verbs = ["test", "examine"],
                    Answer = "You successfully tested the action",
                    ChildId = 2
                }
            ]
        };
    }

    /// <summary>
    /// Creates a simple mock ChoiceNode for testing
    /// </summary>
    public static ChoiceNode CreateMockChoiceNode()
    {
        return new ChoiceNode
        {
            Id = 3,
            Text = "Test Choice Node",
            Choices =
            [
                new() {
                    ChildId = 4
                },
                new() {
                    ChildId = 5
                }
            ]
        };
    }

    /// <summary>
    /// Creates a simple mock DialogueNode for testing
    /// </summary>
    public static DialogueNode CreateMockDialogueNode()
    {
        return new DialogueNode
        {
            Id = 6,
            Text = "Test Dialogue Node",
            ChildId = 7,
            Dialogues =
            [
                new() {
                    Line = "Hello, traveler",
                    LineName = "greeting",
                    NextLine = "response",
                    Break = true
                },
                new() {
                    Line = "How are you today?",
                    LineName = "response",
                    Break = true
                }
            ]
        };
    }

    /// <summary>
    /// Creates a simple mock Chapter for testing
    /// </summary>
    public static Chapter CreateMockChapter()
    {
        ActionNode actionNode = CreateMockActionNode();
        ChoiceNode choiceNode = CreateMockChoiceNode();
        DialogueNode dialogueNode = CreateMockDialogueNode();

        return new Chapter
        {
            Id = 1,
            Title = "Test Chapter",
            Nodes = [actionNode, choiceNode, dialogueNode]
        };
    }
}
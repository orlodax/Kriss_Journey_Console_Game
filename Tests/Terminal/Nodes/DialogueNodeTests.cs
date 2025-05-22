using System;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests.Terminal.Nodes;

[TestClass]
public class DialogueNodeTests : NodeTestBase
{
    private DialogueNode dialogueNode;

    [TestInitialize]
    public override void TestInitialize()
    {
        base.TestInitialize();
        dialogueNode = CreateNode<DialogueNode>(configure: node =>
        {
            node.Id = 1;
            node.Dialogues =
            [
                new Dialogue
                {
                    Actor = "A", Line = "Hello", Break = true, Replies =
                    [
                        new() { Line = "Reply1", ChildId = 2 },
                        new() { Line = "Reply2", NextLine = "L2" }
                    ]
                },
                new Dialogue { Actor = "B", Line = "World", LineName = "L2", ChildId = 3 },
            ];
        });
    }

    [TestMethod]
    public void DisplaysDialogueAndReplies()
    {
        SimulateUserInput(ConsoleKey.Enter);

        LoadNode(dialogueNode);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("Hello"));
        Assert.IsTrue(output.Contains("Reply1"));
        Assert.IsTrue(output.Contains("Reply2"));
    }

    [TestMethod]
    public void SelectingReplyWithChildId_AdvancesToNextNode()
    {
        SimulateUserInput(ConsoleKey.Enter, ConsoleKey.Enter); // one for wait for key, one to select first reply
        _ = CreateNode<StoryNode>(nodeId: 2, configure: n => n.Text = "Next node loaded!");

        LoadNode(dialogueNode);
        // Should have called AdvanceToNext with 2 (would throw if not found)
    }

    [TestMethod]
    public void SelectingReplyWithNextLine_ContinuesDialogue()
    {
        SimulateUserInput(ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.Enter); // select second reply
        _ = CreateNode<StoryNode>(nodeId: 3, configure: n => n.Text = "Next node loaded!");

        LoadNode(dialogueNode);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("World"));
    }

    [TestMethod]
    public void SelectingReplyWithInvalidChildId_Throws()
    {
        // Add a reply with a ChildId that does not exist
        dialogueNode.Dialogues[0].Replies.Add(new Reply { Line = "Ghost", ChildId = 99 });
        SimulateUserInput(ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.Enter); // select "Ghost"

        Assert.ThrowsException<ArgumentNullException>(dialogueNode.Load);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("Hello"));
        Assert.IsTrue(output.Contains("Ghost"));
        TerminalMock.ResetOutput();
    }

    [TestMethod]
    public void DialogueWithNoReplies_ContinuesToNextDialogue()
    {
        // Add a dialogue with no replies, should auto-continue to next
        dialogueNode.Dialogues =
        [
            new Dialogue { Actor = "C", Line = "No replies here" },
            new Dialogue { Actor = "B", Line = "World", LineName = "L2", ChildId = 3 }
        ];
        SimulateUserInput(ConsoleKey.Enter, ConsoleKey.Enter); // Just advance
        _ = CreateNode<StoryNode>(nodeId: 3, configure: n => { n.Text = "Next node loaded!"; n.ChildId = 1; });

        LoadNode(dialogueNode);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("World"));
        Assert.IsTrue(output.Contains("No replies here"));
    }

    [TestMethod]
    public void DialogueWithBreak_WaitsForEnter()
    {
        // Add a dialogue with Break = false, should wait for Enter
        dialogueNode.Dialogues.Insert(0, new Dialogue { Actor = "X", Line = "Keep going", Break = true, Replies = [] });
        SimulateUserInput(ConsoleKey.Enter); // Should advance to the next line

        LoadNode(dialogueNode);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("Keep going"));
        Assert.IsTrue(output.Contains("Hello"));
    }

    [TestMethod]
    public void SelectingReplyWithNextLineAndChildId_RendersNextLineThenAdvancesToChildId()
    {
        dialogueNode.Dialogues[0].Replies.Add(new Reply { Line = "Both", NextLine = "L2", ChildId = 3 });
        _ = CreateNode<StoryNode>(nodeId: 3, configure: n => { n.Text = "Advanced to 3!"; n.ChildId = 1; });
        SimulateUserInput(ConsoleKey.Enter, ConsoleKey.DownArrow, ConsoleKey.DownArrow, ConsoleKey.Enter); // select "Both"

        LoadNode(dialogueNode);

        string output = TerminalMock.GetOutput();
        Assert.IsTrue(output.Contains("World"));
        Assert.IsTrue(output.Contains("Advanced to 3!"));
    }
}
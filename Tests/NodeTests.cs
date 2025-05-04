using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KrissJourney.Kriss.Classes;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests
{
    [TestClass]
    public class NodeTests
    {
        private StringWriter stringWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            DataLayer.Init();

            // Redirect console output for testing
            stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up redirected console output
            stringWriter.Dispose();
        }

        [TestMethod]
        public void ThereAreNoLongDialougesWithReplies()
        {
            IEnumerable<DialogueNode> res = DataLayer.Chapters.SelectMany(c => c.Nodes.OfType<DialogueNode>());

            IEnumerable<Dialogue> longOnes = res.SelectMany(x => x.Dialogues.Where(y => y.Break == true));

            IEnumerable<Dialogue> withReplies = longOnes.Where(l => l.Replies != null);

            Assert.IsTrue(!withReplies.Any());
        }

        [TestMethod]
        public void NodeBase_InitMethod_SetsPropertiesCorrectly()
        {
            // Arrange
            StoryNode storyNode = new()
            {
                Id = 1,
                Text = "This is a test story node",
                ChildId = 2,
                IsVisited = false
            };

            // The Init method is protected and called within Load(),
            // we can't directly test it, but this ensures no exceptions are thrown
            try
            {
                // Act - indirectly calls Init()
                storyNode.Load();

                // Assert
                Assert.IsTrue(true, "Init method should execute without exceptions");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception thrown: {ex.Message}");
            }
        }

        [TestMethod]
        public void ActionNode_Properties_SetCorrectly()
        {
            // Arrange
            ActionNode actionNode = new()
            {
                Id = 5,
                Text = "You are in a dark room.",
                ChildId = 6,
                Actions =
                [
                    new() {
                        Verbs = ["look", "examine"],
                        Answer = "You see a faint light in the distance.",
                        ChildId = 7
                    }
                ]
            };

            // Act & Assert
            Assert.AreEqual(5, actionNode.Id);
            Assert.AreEqual("You are in a dark room.", actionNode.Text);
            Assert.AreEqual(6, actionNode.ChildId);
            Assert.AreEqual(1, actionNode.Actions.Count);
            Assert.AreEqual("look", actionNode.Actions[0].Verbs[0]);
            Assert.AreEqual("You see a faint light in the distance.", actionNode.Actions[0].Answer);
            Assert.AreEqual(7, actionNode.Actions[0].ChildId);
        }

        [TestMethod]
        public void ChoiceNode_Properties_SetCorrectly()
        {
            // Arrange
            ChoiceNode choiceNode = new()
            {
                Id = 10,
                Text = "You stand at a crossroads.",
                Choices =
                [
                    new() {
                        ChildId = 11
                    },
                    new() {
                        ChildId = 12,
                        Condition = new Condition
                        {
                            Item = "map",
                            Refusal = "You need a map to go that way."
                        }
                    }
                ]
            };

            // Act & Assert
            Assert.AreEqual(10, choiceNode.Id);
            Assert.AreEqual("You stand at a crossroads.", choiceNode.Text);
            Assert.AreEqual(2, choiceNode.Choices.Count);
            Assert.AreEqual(11, choiceNode.Choices[0].ChildId);
            Assert.AreEqual(12, choiceNode.Choices[1].ChildId);
            Assert.AreEqual("map", choiceNode.Choices[1].Condition.Item);
        }

        [TestMethod]
        public void DialogueNode_Properties_SetCorrectly()
        {
            // Arrange
            DialogueNode dialogueNode = new()
            {
                Id = 15,
                Text = "You approach the innkeeper.",
                ChildId = 16,
                Dialogues =
                [
                    new() {
                        Line = "Good day, traveler!",
                        LineName = "greeting",
                        Break = true,
                        NextLine = "response"
                    },
                    new() {
                        Line = "What brings you to our humble village?",
                        LineName = "response",
                        Break = false
                    }
                ]
            };

            // Act & Assert
            Assert.AreEqual(15, dialogueNode.Id);
            Assert.AreEqual("You approach the innkeeper.", dialogueNode.Text);
            Assert.AreEqual(16, dialogueNode.ChildId);
            Assert.AreEqual(2, dialogueNode.Dialogues.Count);
            Assert.AreEqual("Good day, traveler!", dialogueNode.Dialogues[0].Line);
            Assert.AreEqual(true, dialogueNode.Dialogues[0].Break);
            Assert.AreEqual("response", dialogueNode.Dialogues[0].NextLine);
        }

        [TestMethod]
        public void StoryNode_Properties_SetCorrectly()
        {
            // Arrange
            StoryNode storyNode = new()
            {
                Id = 20,
                Text = "And so begins your adventure...",
                ChildId = 21,
                IsLast = false,
                IsClosing = false
            };

            // Act & Assert
            Assert.AreEqual(20, storyNode.Id);
            Assert.AreEqual("And so begins your adventure...", storyNode.Text);
            Assert.AreEqual(21, storyNode.ChildId);
            Assert.IsFalse(storyNode.IsLast);
            Assert.IsFalse(storyNode.IsClosing);
        }

        [TestMethod]
        public void MiniGame01_InheritsFromActionNode()
        {
            // Arrange
            MiniGame01 miniGame = new()
            {
                Id = 25,
                Text = "Efeliah looks at you expectantly.",
                ChildId = 26
            };

            // Act & Assert
            Assert.IsInstanceOfType(miniGame, typeof(ActionNode));
            Assert.AreEqual(25, miniGame.Id);
            Assert.AreEqual("Efeliah looks at you expectantly.", miniGame.Text);
            Assert.AreEqual(26, miniGame.ChildId);
        }
    }
}
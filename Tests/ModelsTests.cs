using KrissJourney.Kriss.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests
{
    [TestClass]
    public class ModelsTests
    {
        [TestMethod]
        public void Action_BasicProperties_ShouldWork()
        {
            // Arrange
            Action action = new()
            {
                Verbs = ["take", "grab", "pick"],
                ChildId = 10,
                Answer = "You picked up the item",
                Objects =
                [
                    new ActionObject
                    {
                        Objs = ["sword", "blade"],
                        Answer = "You grabbed the sword",
                        ChildId = 11
                    }
                ],
                Condition = new Condition
                {
                    Item = "key",
                    Refusal = "You need a key to do that"
                },
                Effect = new Effect
                {
                    GainItem = "sword"
                }
            };

            // Act & Assert
            Assert.AreEqual(3, action.Verbs.Count);
            Assert.AreEqual(10, action.ChildId);
            Assert.AreEqual("You picked up the item", action.Answer);
            Assert.AreEqual(1, action.Objects.Count);
            Assert.AreEqual("key", action.Condition.Item);
            Assert.AreEqual("sword", action.Effect.GainItem);

            // Test GetOpinion method
            string opinion = action.GetOpinion("take");
            Assert.IsNotNull(opinion);
        }

        [TestMethod]
        public void Choice_BasicProperties_ShouldWork()
        {
            // Arrange
            Choice choice = new()
            {
                ChildId = 5,
                Condition = new Condition
                {
                    Item = "map",
                    Refusal = "You need a map to go there"
                }
            };

            // Act & Assert
            Assert.AreEqual(5, choice.ChildId);
            Assert.AreEqual("map", choice.Condition.Item);
        }

        [TestMethod]
        public void Dialogue_BasicProperties_ShouldWork()
        {
            // Arrange
            Dialogue dialogue = new()
            {
                Line = "Hello, traveler",
                LineName = "greeting",
                Break = true,
                ChildId = 7,
                NextLine = "response"
            };

            // Act & Assert
            Assert.AreEqual("Hello, traveler", dialogue.Line);
            Assert.AreEqual("greeting", dialogue.LineName);
            Assert.IsTrue(dialogue.Break);
            Assert.AreEqual(7, dialogue.ChildId);
            Assert.AreEqual("response", dialogue.NextLine);
        }

        [TestMethod]
        public void Chapter_BasicProperties_ShouldWork()
        {
            // Arrange
            Chapter chapter = TestUtils.CreateMockChapter();

            // Act & Assert
            Assert.AreEqual(1, chapter.Id);
            Assert.AreEqual("Test Chapter", chapter.Title);
            Assert.AreEqual(3, chapter.Nodes.Count);
        }

        [TestMethod]
        public void Status_BasicProperties_ShouldWork()
        {
            // Arrange
            Status status = new();
            status.Inventory.Add("sword");
            status.Inventory.Add("potion");

            status.VisitedNodes[1] = [1, 2, 3];
            status.VisitedNodes[2] = [10, 11];

            // Act & Assert
            Assert.AreEqual(2, status.Inventory.Count);
            Assert.IsTrue(status.Inventory.Contains("sword"));
            Assert.IsTrue(status.Inventory.Contains("potion"));

            Assert.AreEqual(2, status.VisitedNodes.Count);
            Assert.AreEqual(3, status.VisitedNodes[1].Count);
            Assert.AreEqual(2, status.VisitedNodes[2].Count);
        }
    }
}
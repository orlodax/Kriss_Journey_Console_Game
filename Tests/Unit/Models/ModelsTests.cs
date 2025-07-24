using KrissJourney.Kriss.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests.Unit.Models;

[TestClass]
public class ModelsTests
{
    [TestMethod]
    public void Action_BasicProperties_ShouldWork()
    {
        // Arrange
        Kriss.Models.Action action = new()
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

        // Test GetAnswer method
        string opinion = action.GetAnswer("take");
        Assert.AreEqual(action.Answer, opinion);
    }
}

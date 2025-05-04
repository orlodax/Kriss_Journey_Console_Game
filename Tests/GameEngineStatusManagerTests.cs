using System.Collections.Generic;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrissJourney.Tests;

[TestClass]
public class GameEngineStatusManagerTests
{
    StatusManager statusManager;
    GameEngine gameEngine;

    [TestInitialize]
    public void TestInitialize()
    {
        statusManager = new StatusManager();
        gameEngine = new GameEngine(statusManager);
        gameEngine.Run();
    }

    [TestCleanup]
    public void TestCleanup()
    {

    }

    [TestMethod]
    public void Init_LoadsChapters()
    {
        List<Chapter> chapters = gameEngine.GetChapters();
        Assert.IsNotNull(chapters, "Chapters field should not be null");
        Assert.IsTrue(chapters.Count > 0, "Chapters should be loaded");
    }

    [TestMethod]
    public void Evaluate_WithNullCondition_ReturnsTrue()
    {
        Assert.IsTrue(gameEngine.Evaluate(null), "Null condition should evaluate to true");
    }

    [TestMethod]
    public void Evaluate_WithConditionAndNoRequiredItem_ReturnsTrue()
    {
        Condition condition = new()
        {
            Item = null,
            Refusal = "You can't do that"
        };

        Assert.IsTrue(gameEngine.Evaluate(condition), "Condition with no required item should evaluate to true");
    }

    [TestMethod]
    public void Evaluate_WithEmptyItemCondition_ReturnsTrue()
    {
        // Arrange
        Condition condition = new()
        {
            Type = "default",
            Item = "", // Empty string
            Refusal = "You can't do that"
        };

        // Act
        bool result = gameEngine.Evaluate(condition);

        // Assert
        Assert.IsTrue(result, "Empty item string should evaluate to true");
    }

    [TestMethod]
    public void AddItemToInventory_WithGainItem_AddsItemToInventory()
    {
        // Arrange
        Effect effect = new()
        {
            GainItem = "test_sword"
        };

        // Act
        gameEngine.AddItemToInventory(effect);

        // Assert
        Assert.IsTrue(statusManager.IsItemInInventory("test_sword"), "Item should be added to inventory");
    }

}

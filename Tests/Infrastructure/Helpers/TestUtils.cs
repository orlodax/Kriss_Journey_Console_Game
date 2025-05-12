using System.Collections.Generic;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Nodes;
using KrissJourney.Kriss.Services;
using KrissJourney.Tests.Infrastructure.Mocks;

namespace KrissJourney.Tests.Infrastructure.Helpers;

/// <summary>
/// Extensions and utilities for testing with GameEngine
/// </summary>
public static class GameEngineTestExtensions
{
    /// <summary>
    /// Get the chapters from a GameEngine instance using reflection
    /// </summary>
    public static List<Chapter> GetChapters(this GameEngine gameEngine)
    {
        var chaptersField = typeof(GameEngine).GetField("chapters",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return chaptersField?.GetValue(gameEngine) as List<Chapter>;
    }

    /// <summary>
    /// Setup a GameEngine instance with the default StatusManager
    /// </summary>
    public static GameEngine Setup()
    {
        GameEngine gameEngine = new(new StatusManager());
        gameEngine.Run();
        return gameEngine;
    }

    /// <summary>
    /// Setup a GameEngine instance with a TestStatusManager
    /// </summary>
    public static GameEngine SetupWithTestStatusManager()
    {
        GameEngine gameEngine = new(new TestStatusManager());
        gameEngine.Run();
        return gameEngine;
    }

    /// <summary>
    /// Get the current chapter from a GameEngine instance using reflection
    /// </summary>
    public static Chapter GetCurrentChapter(this GameEngine gameEngine)
    {
        var currentChapterField = typeof(GameEngine).GetField("currentChapter",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return currentChapterField?.GetValue(gameEngine) as Chapter;
    }

    /// <summary>
    /// Get the current node from a GameEngine instance using reflection
    /// </summary>
    public static NodeBase GetCurrentNode(this GameEngine gameEngine)
    {
        var currentNodeField = typeof(GameEngine).GetField("currentNode",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return currentNodeField?.GetValue(gameEngine) as NodeBase;
    }
}

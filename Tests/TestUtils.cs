using System.Collections.Generic;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Services;

namespace KrissJourney.Tests;

public static class GameEngineTestExtensions
{
    public static List<Chapter> GetChapters(this GameEngine gameEngine)
    {
        var chaptersField = typeof(GameEngine).GetField("chapters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return chaptersField?.GetValue(gameEngine) as List<Chapter>;
    }

    public static GameEngine Setup()
    {
        GameEngine gameEngine = new(new StatusManager());
        gameEngine.Run();
        return gameEngine;
    }
}

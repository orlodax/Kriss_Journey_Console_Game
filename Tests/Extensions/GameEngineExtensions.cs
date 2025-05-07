using System.Collections.Generic;
using KrissJourney.Kriss.Models;
using KrissJourney.Kriss.Services;

namespace KrissJourney.Tests.Extensions;

public static class GameEngineExtensions
{
    public static List<Chapter> GetChapters(this GameEngine gameEngine)
    {
        var chaptersField = typeof(GameEngine).GetField("chapters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return chaptersField?.GetValue(gameEngine) as List<Chapter>;
    }
}
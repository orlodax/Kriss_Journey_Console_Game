using System.Text.Json;

namespace KrissJourney.Kriss.Helpers;

public static class JsonHelper
{
    public static JsonSerializerOptions Options =>
        new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };
}

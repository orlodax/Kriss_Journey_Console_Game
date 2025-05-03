using KrissJourney.Kriss.Nodes;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KrissJourney.Kriss.Classes;

public class NodeJsonConverter : JsonConverter<NodeBase>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(NodeBase).IsAssignableFrom(typeToConvert);
    }

    public override NodeBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // First deserialize to a temporary object to extract the Type property
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement root = document.RootElement;

        // Check if the Type property exists and read its value
        if (!root.TryGetProperty("type", out JsonElement typeProperty))
        {
            throw new JsonException("JSON object does not contain a Type property");
        }

        string nodeType = typeProperty.GetString().ToLowerInvariant();
        string json = root.GetRawText();

        // Directly deserialize to the final implementation class
        return nodeType switch
        {
            "story" => JsonSerializer.Deserialize<StoryNode>(json, options),
            "choice" => JsonSerializer.Deserialize<ChoiceNode>(json, options),
            "dialogue" => JsonSerializer.Deserialize<DialogueNode>(json, options),
            "action" => JsonSerializer.Deserialize<ActionNode>(json, options),
            "minigame01" => JsonSerializer.Deserialize<MiniGame01>(json, options),
            _ => throw new JsonException($"Unknown node type: {nodeType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, NodeBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
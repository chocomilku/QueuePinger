using System.Text.Json.Serialization;

namespace Core.Models;

public record ChatMessage(
    [property: JsonPropertyName("channel_id")]
    int ChannelId,
    [property: JsonPropertyName("content")]
    string Content,
    [property: JsonPropertyName("is_action")]
    bool IsAction,
    [property: JsonPropertyName("message_id")]
    int MessageId,
    [property: JsonPropertyName("sender_id")]
    int SenderId,
    [property: JsonPropertyName("timestamp")]
    DateTime Timestamp,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("uuid")] string? Uuid
);
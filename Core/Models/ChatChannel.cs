using System.Text.Json.Serialization;

namespace Core.Models;

public record PmResponse(
    ChatChannel Channel,
    ChatMessage Message
);

public record PmRequestBody(
    [property: JsonPropertyName("target_id")]
    int TargetId,
    [property: JsonPropertyName("message")]
    string Message
);

public record ChatChannel(
    [property: JsonPropertyName("channel_id")]
    int ChannelId,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("message_length_limit")]
    int MessageLengthLimit,
    [property: JsonPropertyName("moderated")]
    bool Moderated,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] ChannelType Type,
    [property: JsonPropertyName("uuid")] string? Uuid
);

public enum ChannelType
{
    PUBLIC,
    PRIVATE,
    MULTIPLAYER,
    SPECTATOR,
    TEMPORARY,
    PM,
    GROUP,
    ANNOUNCE
}
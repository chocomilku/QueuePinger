using System.Text.Json.Serialization;

namespace Core.Models;

public record MessageTemplate(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("messages")]
    LinkedList<string> MarkdownMessage
);
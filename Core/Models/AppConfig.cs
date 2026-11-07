using System.Text.Json.Serialization;

namespace Core.Models;

public record AppConfig(
    [property: JsonPropertyName("client_id")]
    int ClientId,
    [property: JsonPropertyName("client_secret")]
    string ClientSecret,
    [property: JsonPropertyName("auth_port")]
    int AuthPort,
    [property: JsonPropertyName("auth_timeout")]
    int AuthTimeoutSeconds
);
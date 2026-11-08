using System.Text.Json.Serialization;

namespace Core.Models;

public record User(
    [property: JsonPropertyName("avatar_url")]
    string AvatarUrl,
    [property: JsonPropertyName("country_code")]
    string CountryCode,
    [property: JsonPropertyName("default_group")]
    string? DefaultGroup,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("is_active")]
    bool IsActive,
    [property: JsonPropertyName("is_bot")] bool IsBot,
    [property: JsonPropertyName("is_deleted")]
    bool IsDeleted,
    [property: JsonPropertyName("is_online")]
    bool IsOnline,
    [property: JsonPropertyName("is_supporter")]
    bool IsSupporter,
    [property: JsonPropertyName("last_visit")]
    DateTime? LastVisit,
    [property: JsonPropertyName("pm_friends_only")]
    bool PmFriendsOnly,
    [property: JsonPropertyName("profile_colour")]
    string ProfileColour,
    [property: JsonPropertyName("username")]
    string Username
);
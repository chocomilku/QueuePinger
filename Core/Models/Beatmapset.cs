using System.Text.Json.Serialization;

namespace Core.Models;

public record Beatmapset(
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("artist_unicode")]
    string ArtistUnicode,
    [property: JsonPropertyName("creator")]
    string Creator,
    [property: JsonPropertyName("favourite_count")]
    int FavouriteCount,
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("play_count")]
    int PlayCount,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("spotlight")]
    bool Spotlight,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("title_unicode")]
    string TitleUnicode,
    [property: JsonPropertyName("track_id")]
    int? TrackId,
    [property: JsonPropertyName("user_id")]
    int UserId
);
using System.Text.RegularExpressions;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Processing;

public partial class MapsetResolver(IOsuClient osuClient) : IMapsetResolver
{
    public async Task<MapsetSubmission?> ResolveMapset(SubmissionDetail submission)
    {
        var parsedId = ParseMapsetLink(submission.MapsetLink);
        if (parsedId is null) return null;

        try
        {
            var resolvedBeatmapset = await osuClient.GetBeatmapset(parsedId.Value);

            return new MapsetSubmission(submission, resolvedBeatmapset);
        }
        catch (Exception)
        {
            return null;
            // expound this further if you need more specific error messages
        }
    }

    public int? ParseMapsetLink(string rawMapsetLink)
    {
        var match = BeatmapsetRegex().Match(rawMapsetLink);
        if (!match.Success) return null;

        if (int.TryParse(match.Value.Split('/')[1], out var parsed))
            return parsed;
        else return null;
    }

    [GeneratedRegex(@"beatmapsets\/(\d+)")]
    private static partial Regex BeatmapsetRegex();
}
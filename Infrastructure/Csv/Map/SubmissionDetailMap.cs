using System.Text.RegularExpressions;
using Core.Models;
using CsvHelper;

namespace Infrastructure.Csv.Map;

[CsvRecordMap(typeof(SubmissionDetail))]
internal partial class SubmissionDetailMap
{
    [CsvColumn("Link")]
    [CsvValidate(nameof(IsValidLink), Message = "Invalid Link.")]
    public string MapsetLink { get; set; } = "";

    [CsvColumn("Status")] public SubmissionStatus Status { get; set; }

    [CsvColumn("Remarks")] public string? Remarks { get; set; }

    private static bool IsValidLink(string link)
    {
        return BeatmapsetRegex().IsMatch(link);
    }

    [GeneratedRegex(@"beatmapsets\/(\d+)")]
    private static partial Regex BeatmapsetRegex();
}
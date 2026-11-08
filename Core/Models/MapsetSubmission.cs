namespace Core.Models;

public enum SubmissionStatus
{
    Pending,
    Accept,
    Reject
}

public record SubmissionDetail(
    string MapsetLink,
    SubmissionStatus Status,
    string? Remarks);

public record MapsetSubmission(
    string MapsetLink,
    SubmissionStatus Status,
    string? Remarks,
    int OnlineId,
    string Artist,
    string Title,
    string Creator,
    int UserId
)
{
    public MapsetSubmission(MapsetSubmission submission, Beatmapset beatmapset) : this(submission.MapsetLink,
        submission.Status,
        submission.Remarks, beatmapset.Id, beatmapset.Artist,
        beatmapset.Title, beatmapset.Creator, beatmapset.UserId)
    {
    }
}
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
    public MapsetSubmission(MapsetSubmission submission, Mapset mapset) : this(submission.MapsetLink, submission.Status,
        submission.Remarks, mapset.OnlineId, mapset.Artist,
        mapset.Title, mapset.Creator, mapset.UserId)
    {
    }
}
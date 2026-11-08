using Core.Models;

namespace Core.Interfaces;

public interface IMapsetResolver
{
    Task<MapsetSubmission?> ResolveMapset(SubmissionDetail submission);
    int? ParseMapsetLink(string rawMapsetLink);
}
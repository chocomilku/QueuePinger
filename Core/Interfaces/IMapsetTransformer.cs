using Core.Models;

namespace Core.Interfaces;

public interface IMapsetTransformer
{
    MapsetSubmission ResolveMapset(SubmissionDetail submission);
    string? ParseMapsetLink(string rawMapsetLink);
}
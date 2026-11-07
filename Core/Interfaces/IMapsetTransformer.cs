using Core.Models;

namespace Core.Interfaces;

public interface IMapsetTransformer
{
    Mapset ResolveMapset(int onlineId);
    string? ParseMapsetLink(string rawMapsetLink);
}
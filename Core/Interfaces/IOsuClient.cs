using Core.Models;

namespace Core.Interfaces;

public interface IOsuClient
{
    Task<Beatmapset> GetBeatmapset(int setId);
    Task<PmResponse> SendToUser(int userId, string message);
}
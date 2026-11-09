using Core.Models;

namespace Core.Interfaces;

public interface IOsuClient
{
    Task<Beatmapset> GetBeatmapset(int setId);
    Task SendToUser(int userId, string message);
}
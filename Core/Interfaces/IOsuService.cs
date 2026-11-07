using Core.Models;

namespace Core.Interfaces;

public interface IOsuService
{
    Mapset GetBeatMapset(int onlineId);
    void CreateChatChannel(); // TODO: actual model for this
    void SendToChannel(); // TODO: actual model for this
}
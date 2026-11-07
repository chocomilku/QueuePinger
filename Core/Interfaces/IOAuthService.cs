namespace Core.Interfaces;

public interface IOAuthService
{
    void StartHttpServer(int port);
    void CloseHttpServer();
    string GenerateAuthLink();
    (string, string) RegisterToken();
}
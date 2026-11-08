namespace Core.Interfaces;

public interface ITokenStore
{
    void SetBearerToken(string token);
    string? GetBearerToken();
}
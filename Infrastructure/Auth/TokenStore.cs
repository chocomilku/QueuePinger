using Core.Interfaces;

namespace Infrastructure.Auth;

public class TokenStore : ITokenStore
{
    private string? _bearerToken = null;

    public void SetBearerToken(string token)
    {
        _bearerToken = token;
    }

    public string? GetBearerToken()
    {
        return _bearerToken;
    }
}
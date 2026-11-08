using Core.Interfaces;

namespace Infrastructure.Auth;

public class TokenStore : ITokenStore
{
    public string? BearerToken { get; set; }
}
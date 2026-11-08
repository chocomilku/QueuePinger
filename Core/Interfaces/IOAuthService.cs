using Core.Models;

namespace Core.Interfaces;

public interface IOAuthService
{
    Task<string> AuthenticateAsync();
    Task<OAuthToken> ExchangeToToken(string code);
    string GenerateOAuthGrant();
}
namespace Core.Interfaces;

public interface ITokenStore
{
    string? BearerToken { get; set; }
}
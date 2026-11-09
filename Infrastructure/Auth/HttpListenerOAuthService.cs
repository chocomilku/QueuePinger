using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Auth;

public class HttpListenerOAuthService(IHttpClientFactory httpClientFactory, IAppConfigService appConfig) : IOAuthService
{
    private readonly IAppConfigService _appConfig = appConfig;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private AppConfig Config =>
        _appConfig.GetConfig() ?? throw new InvalidOperationException("OAuthService: Config is empty.");

    public async Task<string> AuthenticateAsync()
    {
        var redirectUri = $"http://localhost:{Config.AuthPort}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(redirectUri);

        try
        {
            listener.Start();
        }
        catch (HttpListenerException ex)
        {
            throw new InvalidOperationException(
                $"Could not start local listener on {redirectUri}." +
                $"The port may be in use, or you may need URL reservation permissions. Try a different port or run as administrator.",
                ex);
        }

        var context = await listener.GetContextAsync();
        var queryParams = HttpUtility.ParseQueryString(context.Request.Url?.Query ?? string.Empty);
        var code = queryParams["code"];

        var response = @"OK";
        var buffer = Encoding.UTF8.GetBytes(response);
        context.Response.ContentLength64 = buffer.Length;
        context.Response.ContentType = "text/html; charset=utf-8";

        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.OutputStream.Close();

        listener.Stop();

        return string.IsNullOrEmpty(code) ? throw new InvalidOperationException("Invalid OAuth code.") : code;
    }

    public async Task<OAuthToken> ExchangeToToken(string code)
    {
        var client = _httpClientFactory.CreateClient("OsuOAuthClient");

        var payload = new Dictionary<string, string>
        {
            { "client_id", Config.ClientId.ToString() },
            { "client_secret", Config.ClientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_url", $"http://localhost:{Config.AuthPort}/" }
        };

        var response = await client.PostAsync("oauth/token", new FormUrlEncodedContent(payload));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OAuthToken>() ?? throw new InvalidOperationException();
    }

    public string GenerateOAuthGrant()
    {
        var builder = new UriBuilder("https://osu.ppy.sh")
        {
            Path = "oauth/authorize"
        };

        var query = new Dictionary<string, string>
        {
            { "client_id", Config.ClientId.ToString() },
            { "redirect_url", $"http://localhost:{Config.AuthPort}/" },
            { "response_type", "code" },
            { "scope", "public chat.write chat.write_manage identify" }
        };

        return QueryHelpers.AddQueryString(builder.ToString(), query!);
    }
}
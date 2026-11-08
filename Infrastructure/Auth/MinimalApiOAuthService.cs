using System.Diagnostics;
using System.Net.Http.Json;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Auth;

public class MinimalApiOAuthService(IHttpClientFactory httpClientFactory, IAppConfigService appConfig) : IOAuthService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private readonly AppConfig _config =
        appConfig.GetConfig() ?? throw new InvalidOperationException("OAuthService: Config is empty.");

    public async Task<string> AuthenticateAsync()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseUrls($"http://localhost:{_config.AuthPort}");
        var server = builder.Build();

        var tcs = new TaskCompletionSource<string>();

        server.MapGet("/auth/callback", async context =>
        {
            if (context.Request.Query.TryGetValue("code", out var code) && !string.IsNullOrEmpty(code))
            {
                tcs.SetResult(code!);
                await context.Response.WriteAsync("OK");
            }
            else
            {
                tcs.SetException(new InvalidOperationException("OAuthService: Invalid Auth Code."));
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid Auth Code");
            }
        });

        await server.StartAsync();

        try
        {
            var code = await tcs.Task;
            return code;
        }
        finally
        {
            await server.StopAsync();
            await server.DisposeAsync();
        }
    }

    public async Task<OAuthToken> ExchangeToToken(string code)
    {
        var client = _httpClientFactory.CreateClient("OsuOAuthClient");

        var payload = new Dictionary<string, string>
        {
            { "client_id", _config.ClientId.ToString() },
            { "client_secret", _config.ClientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_url", $"http://localhost:{_config.AuthPort}/auth/callback" }
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
            { "client_id", _config.ClientId.ToString() },
            { "redirect_url", $"http://localhost:{_config.AuthPort}/auth/callback" },
            { "response_type", "code" },
            { "scope", "public chat.write chat.write_manage identify" }
        };

        return QueryHelpers.AddQueryString(builder.ToString(), query!);
    }
}
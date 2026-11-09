using System.Net.Http.Json;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Api;

public class OsuClient(IHttpClientFactory httpClientFactory) : IOsuClient
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<Beatmapset> GetBeatmapset(int setId)
    {
        var client = _httpClientFactory.CreateClient("OsuClient");

        var response = await client.GetAsync($"api/v2/beatmapsets/{setId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Beatmapset>() ?? throw new InvalidOperationException();
    }

    public async Task SendToUser(int userId, string message)
    {
        var client = _httpClientFactory.CreateClient("OsuClient");

        var response = await client.PostAsJsonAsync($"api/v2/chat/new", new PmRequestBody(userId, message));

        response.EnsureSuccessStatusCode();
    }
}
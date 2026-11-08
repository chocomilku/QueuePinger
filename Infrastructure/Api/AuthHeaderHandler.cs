using System.Net.Http.Headers;
using Core.Interfaces;

namespace Infrastructure.Api;

public class AuthHeaderHandler(ITokenStore tokenStore) : DelegatingHandler
{
    private readonly ITokenStore _tokenStore = tokenStore;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _tokenStore.BearerToken;
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    // TODO: Attach .AddHttpMessageHandler<AuthHeaderHandler>(); to AddHttpClient DI thing 
}
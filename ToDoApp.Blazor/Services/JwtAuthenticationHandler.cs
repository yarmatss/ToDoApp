using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using ToDoApp.Shared.Services;

namespace ToDoApp.Blazor.Services;

public class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly IAuthClient _authClient;

    public JwtAuthenticationHandler(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.RequestUri.AbsolutePath.Contains("/api/Auth/refresh"))
        {
            var accessToken = await _authClient.GetAccessTokenAsync();

            if (!string.IsNullOrEmpty(accessToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                if (jwtToken.ValidTo <= DateTime.UtcNow)
                {
                    var refreshed = await _authClient.RefreshTokensAsync();
                    if (refreshed)
                    {
                        accessToken = await _authClient.GetAccessTokenAsync();
                    }
                    else
                    {
                        await _authClient.ClearTokensAsync();
                    }
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
using CommunityToolkit.Maui.Converters;
using System.Net.Http.Headers;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.Services;

public partial class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly IAuthClient _authClient;

    public JwtAuthenticationHandler(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isExpired = await _authClient.IsTokenExpiredAsync();
        if (isExpired)
        {
            var refreshed = await _authClient.RefreshTokensAsync();
            if (!refreshed)
            {
                await _authClient.ClearTokensAsync();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync("/login");
                });
                //return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }

        if (string.IsNullOrEmpty(await _authClient.GetAccessTokenAsync()))
        {
            await Shell.Current.GoToAsync("/login");
        }

        var accessToken = await _authClient.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

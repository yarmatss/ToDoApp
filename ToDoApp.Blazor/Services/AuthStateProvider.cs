using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDoApp.Shared.Services;

namespace ToDoApp.Blazor.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthClient _authClient;

    public AuthStateProvider(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await _authClient.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(accessToken);
        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");

        var state = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
}

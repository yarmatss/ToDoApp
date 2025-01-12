using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Shared.Services;

namespace ToDoApp.MVC.Controllers;

public class AuthController : Controller
{
    private readonly IApiClient _apiClient;
    private readonly IAuthStateService _authState;

    public AuthController(IApiClient apiClient, IAuthStateService authState)
    {
        _apiClient = apiClient;
        _authState = authState;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _apiClient.RegisterAsync(model);
            await SetAuthenticationCookies(result);
            return RedirectToAction("Index", "Todo");
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Registration error.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _apiClient.LoginAsync(model);
            await SetAuthenticationCookies(result);
            _authState.SetTokens(result.AccessToken, result.RefreshToken);
            return Redirect(returnUrl ?? "/");
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Login");
    }

    private async Task SetAuthenticationCookies(TokenDto token)
    {
        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, token.AccessToken),
                    new Claim("RefreshToken", token.RefreshToken)
                };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });

        Response.Cookies.Append("JWTToken", token.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}

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
    private readonly IAuthClient _authClient;

    public AuthController(IApiClient apiClient, IAuthClient authClient)
    {
        _apiClient = apiClient;
        _authClient = authClient;
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
            await _authClient.SetTokensAsync(result.AccessToken, result.RefreshToken);

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email)
                // Add additional claims if necessary
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

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

            // Store tokens using AuthClient
            await _authClient.SetTokensAsync(result.AccessToken, result.RefreshToken);

            // Create user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email)
                // Add additional claims if necessary
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

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
        await _authClient.ClearTokensAsync();
        return RedirectToAction("Login");
    }
}

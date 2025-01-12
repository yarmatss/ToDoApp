using Microsoft.AspNetCore.Authentication.Cookies;
using ToDoApp.MVC.Auth;
using ToDoApp.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthClient, AuthClient>();

builder.Services.AddTransient<JwtAuthenticationHandler>();

// Register a named HttpClient for AuthClient without JwtAuthenticationHandler
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Register the main ApiClient with JwtAuthenticationHandler
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
})
.AddHttpMessageHandler<JwtAuthenticationHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
       {
           options.LoginPath = "/Auth/Login";
           options.LogoutPath = "/Auth/Logout";
           options.AccessDeniedPath = "/Auth/AccessDenied";
           options.ExpireTimeSpan = TimeSpan.FromHours(1);
           options.SlidingExpiration = true;
       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Index}/{id?}");

app.Run();

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ToDoApp.MAUI.Pages.Auth;
using ToDoApp.MAUI.Pages.Todo;
using ToDoApp.MAUI.Services;
using ToDoApp.MAUI.ViewModels.Auth;
using ToDoApp.MAUI.ViewModels.Todo;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IAuthClient, AuthClient>();

        builder.Services.AddTransient<JwtAuthenticationHandler>();

        string baseAddress = DeviceInfo.Platform == DevicePlatform.Android ?
            "https://10.0.2.2:7059/" : "https://localhost:7059/";
        builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
        .AddHttpMessageHandler<JwtAuthenticationHandler>();

        builder.Services.AddHttpClient<IAuthClient, AuthClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<TodoListPage>();
        builder.Services.AddTransient<TodoDetailsPage>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<TodoListViewModel>();
        builder.Services.AddTransient<TodoDetailsViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

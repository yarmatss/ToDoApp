using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IAuthClient _authClient;

    public App(IAuthClient authClient)
    {
        InitializeComponent();
        _authClient = authClient;
        CheckAuthenticationAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell(_authClient));
    }

    private async void CheckAuthenticationAsync()
    {
        var token = await _authClient.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            await Shell.Current.GoToAsync("login");
        }
    }
}
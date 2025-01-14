using CommunityToolkit.Mvvm.Input;
using ToDoApp.MAUI.Pages.Auth;
using ToDoApp.MAUI.Pages.Todo;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI;

public partial class AppShell : Shell
{
    private readonly IAuthClient _authClient;

    public AppShell(IAuthClient authClient)
    {
        InitializeComponent();
        RegisterRoutes();
        _authClient = authClient;
        BindingContext = this;
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("todolist", typeof(TodoListPage));
        Routing.RegisterRoute("todolist/details", typeof(TodoDetailsPage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authClient.ClearTokensAsync();
        await GoToAsync("///login");
    }
}

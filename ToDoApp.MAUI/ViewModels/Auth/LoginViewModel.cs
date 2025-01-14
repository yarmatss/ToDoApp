using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.MAUI.Services;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiClient _apiClient;
    private readonly IAuthClient _authClient;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    public LoginViewModel(
                        IApiClient apiClient,
                        IAuthClient authClient,
                        INavigationService navigationService,
                        IDialogService dialogService)
    {
        _apiClient = apiClient;
        _authClient = authClient;
        _navigationService = navigationService;
        _dialogService = dialogService;
        Title = "Login";
    }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [RelayCommand]
    async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
        {
            await _dialogService.ShowAlertAsync("Error", "Please fill all fields", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var loginDto = new LoginDto { Email = Email, Password = Password };
            var result = await _apiClient.LoginAsync(loginDto);
            await _authClient.SetTokensAsync(result.AccessToken, result.RefreshToken);
            await _navigationService.NavigateToAsync("///todolist");
        });
    }

    [RelayCommand]
    async Task GoToRegisterAsync()
    {
        await _navigationService.NavigateToAsync("///register");
    }
}
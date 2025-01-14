using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.MAUI.Services;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IApiClient _apiClient;
    private readonly IAuthClient _authClient;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string confirmPassword;

    public RegisterViewModel(IApiClient apiClient, IAuthClient authClient,
        INavigationService navigationService, IDialogService dialogService)
    {
        _apiClient = apiClient;
        _authClient = authClient;
        _navigationService = navigationService;
        _dialogService = dialogService;
        Title = "Register";
    }

    [RelayCommand]
    async Task RegisterAsync()
    {
        if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
        {
            await _dialogService.ShowAlertAsync("Error", "Please fill all fields", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await _dialogService.ShowAlertAsync("Error", "Passwords do not match", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var registerDto = new RegisterDto { Email = Email, Password = Password, ConfirmPassword = ConfirmPassword };
            var result = await _apiClient.RegisterAsync(registerDto);
            await _authClient.SetTokensAsync(result.AccessToken, result.RefreshToken);
            await _navigationService.NavigateToAsync("///todolist");
        });
    }

    [RelayCommand]
    async Task GoToLoginAsync()
    {
        await _navigationService.NavigateToAsync("///login");
    }
}

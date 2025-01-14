using ToDoApp.MAUI.ViewModels.Auth;

namespace ToDoApp.MAUI.Pages.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}
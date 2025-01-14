using ToDoApp.MAUI.ViewModels.Auth;

namespace ToDoApp.MAUI.Pages.Auth;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
    }
}
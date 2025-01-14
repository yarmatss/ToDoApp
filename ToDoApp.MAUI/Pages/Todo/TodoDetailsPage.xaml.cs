using ToDoApp.MAUI.ViewModels.Todo;

namespace ToDoApp.MAUI.Pages.Todo;

public partial class TodoDetailsPage : ContentPage
{
    private TodoDetailsViewModel ViewModel => BindingContext as TodoDetailsViewModel;

    public TodoDetailsPage(TodoDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }
}
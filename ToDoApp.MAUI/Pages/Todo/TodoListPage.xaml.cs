using ToDoApp.MAUI.ViewModels.Todo;

namespace ToDoApp.MAUI.Pages.Todo;

public partial class TodoListPage : ContentPage
{
    private TodoListViewModel ViewModel => BindingContext as TodoListViewModel;

    public TodoListPage(TodoListViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeCommand.ExecuteAsync(null);
    }
}
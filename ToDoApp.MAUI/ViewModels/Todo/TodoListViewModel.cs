using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Todo;
using ToDoApp.MAUI.Services;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.ViewModels.Todo;

public partial class TodoListViewModel : BaseViewModel
{
    private readonly IApiClient _apiClient;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<TodoDto> items;

    [ObservableProperty]
    private bool isLoadingMore;

    [ObservableProperty]
    private TodoParameters parameters;

    [ObservableProperty]
    private bool hasMoreItems;

    public TodoListViewModel(IApiClient apiClient, IDialogService dialogService,
                        INavigationService navigationService)
    {
        _apiClient = apiClient;
        _dialogService = dialogService;
        Items = new ObservableCollection<TodoDto>();
        Parameters = new TodoParameters();
        Title = "Lista zadań";
        _navigationService = navigationService;
        InitializeAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        Parameters.PageNumber = 1;
        await LoadDataAsync(true);
    }

    [RelayCommand]
    public async Task LoadMoreAsync()
    {
        if (IsBusy || IsLoadingMore || !HasMoreItems)
            return;

        IsLoadingMore = true;
        Parameters.PageNumber++;
        await LoadDataAsync(false);
        IsLoadingMore = false;
    }

    private async Task LoadDataAsync(bool refresh)
    {
        await ExecuteAsync(async () =>
        {
            var result = await _apiClient.GetTodosAsync(Parameters);

            if (refresh)
                Items.Clear();

            foreach (var item in result.Items)
                Items.Add(item);

            HasMoreItems = result.HasNextPage;
        });
    }

    [RelayCommand]
    private async Task DeleteTodoAsync(TodoDto todo)
    {
        var confirmed = await _dialogService.ShowConfirmationAsync(
            "Delete",
            "Are you sure you want to delete this item?");

        if (!confirmed) return;

        await ExecuteAsync(async () =>
        {
            await _apiClient.DeleteTodoAsync(todo.Id);
            Items.Remove(todo);
        });
    }

    [RelayCommand]
    private async Task EditTodoAsync(TodoDto todo)
    {
        var parameters = new Dictionary<string, object>
        {
            { "id", todo.Id.ToString() },
        };
        await Shell.Current.GoToAsync("///todolist/details", parameters);
    }

    [RelayCommand]
    private async Task AddTodoAsync()
    {
        await _navigationService.NavigateToAsync("///todolist/details");
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDoApp.Application.DTOs.Category;
using ToDoApp.Application.DTOs.Todo;
using ToDoApp.Domain.Enums;
using ToDoApp.MAUI.Services;
using ToDoApp.Shared.Services;

namespace ToDoApp.MAUI.ViewModels.Todo;

[QueryProperty(nameof(TodoIdString), "id")]
public partial class TodoDetailsViewModel : BaseViewModel
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    private Guid? TodoId => !string.IsNullOrEmpty(TodoIdString) ?
        Guid.Parse(TodoIdString) : null;

    [ObservableProperty]
    private string todoIdString;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private DateTime? deadline;

    [ObservableProperty]
    private Priority priority;

    [ObservableProperty]
    private Guid categoryId;

    [ObservableProperty]
    private bool isCompleted;

    [ObservableProperty]
    private ObservableCollection<Priority> priorities;

    [ObservableProperty]
    private CategoryDto selectedCategory;

    [ObservableProperty]
    private ObservableCollection<CategoryDto> categories;

    public TodoDetailsViewModel(IApiClient apiClient, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
        Categories = new ObservableCollection<CategoryDto>();
        Priorities = new ObservableCollection<Priority>(Enum.GetValues<Priority>());
    }

    [RelayCommand]
    public async Task InitializeAsync()
    {
        if (IsBusy) return;

        await ExecuteAsync(async () =>
        {
            await LoadCategoriesAsync();
            if (TodoId.HasValue)
            {
                var todo = await _apiClient.GetTodoByIdAsync(TodoId.Value);
                Title = todo.Title;
                Description = todo.Description;
                Deadline = todo.Deadline;
                Priority = todo.Priority;
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == todo.CategoryId);
            }
        });
    }

    private async Task LoadCategoriesAsync()
    {
        var result = await _apiClient.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in result)
        {
            Categories.Add(category);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(Title))
        {
            await Shell.Current.DisplayAlert("Błąd", "Tytuł jest wymagany", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (TodoId.HasValue)
            {
                var updateDto = new UpdateTodoDto
                {
                    Id = TodoId.Value,
                    Title = Title,
                    Description = Description,
                    Deadline = Deadline,
                    Priority = Priority,
                    CategoryId = SelectedCategory?.Id
                };
                await _apiClient.UpdateTodoAsync(TodoId.Value, updateDto);
            }
            else
            {
                var createDto = new CreateTodoDto
                {
                    Title = Title,
                    Description = Description,
                    Deadline = Deadline,
                    Priority = Priority,
                    CategoryId = SelectedCategory?.Id ?? Guid.Empty
                };
                await _apiClient.CreateTodoAsync(createDto);
            }

            await _navigationService.GoBackAsync();
        });
    }
}
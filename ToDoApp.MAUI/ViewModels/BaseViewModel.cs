using CommunityToolkit.Mvvm.ComponentModel;

namespace ToDoApp.MAUI.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private string errorMessage;

    public bool IsNotBusy => !IsBusy;

    protected async Task ExecuteAsync(Func<Task> operation, bool showLoading = true)
    {
        try
        {
            if (showLoading)
                IsBusy = true;
            ErrorMessage = string.Empty;
            await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            if (showLoading)
                IsBusy = false;
        }
    }

    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, bool showLoading = true)
    {
        try
        {
            if (showLoading)
                IsBusy = true;
            ErrorMessage = string.Empty;
            return await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            return default;
        }
        finally
        {
            if (showLoading)
                IsBusy = false;
        }
    }
}

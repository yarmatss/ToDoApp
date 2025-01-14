namespace ToDoApp.MAUI.Services;

public class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string button)
    {
        await Shell.Current.DisplayAlert(title, message, button);
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return await Shell.Current.DisplayAlert(title, message, accept, cancel);
    }
}
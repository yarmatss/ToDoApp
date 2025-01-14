namespace ToDoApp.MAUI.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string button);
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");
}

namespace ToDoApp.MAUI.Services;

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters = null)
    {
        if (parameters != null)
            await Shell.Current.GoToAsync(route, parameters);
        else
            await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("///login");
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
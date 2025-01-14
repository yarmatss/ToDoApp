﻿namespace ToDoApp.MAUI.Services;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object> parameters = null);
    Task GoBackAsync();
    Task NavigateToLoginAsync();
}
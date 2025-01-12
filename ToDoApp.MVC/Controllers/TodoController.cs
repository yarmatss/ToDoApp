using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Todo;
using ToDoApp.Shared.Services;

namespace ToDoApp.MVC.Controllers;

[Authorize]
public class TodoController : Controller
{
    private readonly IApiClient _apiClient;

    public TodoController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index([FromQuery] TodoParameters parameters)
    {
        try
        {
            var todos = await _apiClient.GetTodosAsync(parameters);
            ViewBag.Categories = await _apiClient.GetCategoriesAsync();
            return View(todos);
        }
        catch
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var todo = await _apiClient.GetTodoByIdAsync(id);
        if (todo == null)
        {
            return NotFound();
        }
        return View(todo);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _apiClient.GetCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(new CreateTodoDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _apiClient.CreateTodoAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var todo = await _apiClient.GetTodoByIdAsync(id);
        if (todo == null)
            return NotFound();

        var dto = new UpdateTodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            Deadline = todo.Deadline,
            Priority = todo.Priority,
            IsCompleted = todo.IsCompleted,
            CategoryId = todo.CategoryId
        };

        ViewBag.Categories = await _apiClient.GetCategoriesAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateTodoDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _apiClient.UpdateTodoAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _apiClient.DeleteTodoAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(Guid id)
    {
        await _apiClient.ToggleCompleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
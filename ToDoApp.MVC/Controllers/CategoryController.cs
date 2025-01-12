using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.DTOs.Category;
using ToDoApp.Shared.Services;

namespace ToDoApp.MVC.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly IApiClient _apiClient;

    public CategoryController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _apiClient.GetCategoriesAsync();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View(new CategoryDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _apiClient.CreateCategoryAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _apiClient.GetCategoryByIdAsync(id);
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, CategoryDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _apiClient.UpdateCategoryAsync(id, dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _apiClient.DeleteCategoryAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
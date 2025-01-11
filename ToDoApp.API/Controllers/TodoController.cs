using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Common.Models;
using ToDoApp.Application.DTOs.Todo;
using ToDoApp.Application.Interfaces;

namespace ToDoApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoController : BaseController
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<TodoDto>>> GetAll([FromQuery] TodoParameters parameters)
    {
        var todos = await _todoService.GetAllAsync(UserId, parameters);
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoDto>> GetById(Guid id)
    {
        var todo = await _todoService.GetByIdAsync(id, UserId);
        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> Create(CreateTodoDto dto)
    {
        var todo = await _todoService.CreateAsync(dto, UserId);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoDto>> Update(Guid id, UpdateTodoDto dto)
    {
        var todo = await _todoService.UpdateAsync(id, dto, UserId);
        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _todoService.DeleteAsync(id, UserId);
        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<TodoDto>> ToggleComplete(Guid id)
    {
        var todo = await _todoService.ToggleCompleteAsync(id, UserId);
        return Ok(todo);
    }
}

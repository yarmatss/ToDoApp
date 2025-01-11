using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Common.Exceptions;

namespace ToDoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) is string id
        ? Guid.Parse(id)
        : throw new UnauthorizedException("User not authenticated");
}

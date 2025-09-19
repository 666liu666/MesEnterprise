using MesEnterprise.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class MesControllerBase : ControllerBase
{
    protected IActionResult Success<T>(T data, string? message = null) => Ok(ApiResponse.Ok(data, message));
    protected IActionResult Failure(string message, string? code = null, int statusCode = StatusCodes.Status400BadRequest)
    {
        return StatusCode(statusCode, ApiResponse.Fail<object?>(message, code));
    }
}

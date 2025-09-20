using MediatR;
using MesEnterprise.Application.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.Auth;

[AllowAnonymous]
[Route("api/v{version:apiVersion}/auth")]
[ApiController]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
}

using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.MoManager;

[Route("api/v{version:apiVersion}/mo-manager")]
[ApiVersion("1.0")]
public class MoManagerController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var data = new { module = "MoManager", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

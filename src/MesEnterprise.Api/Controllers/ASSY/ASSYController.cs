using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.ASSY;

[Route("api/v{version:apiVersion}/assy")]
[ApiVersion("1.0")]
public class ASSYController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var data = new { module = "ASSY", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.KIT;

[Route("api/v{version:apiVersion}/kit")]
[ApiVersion("1.0")]
public class KITController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var data = new { module = "KIT", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

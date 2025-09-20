using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.Route;

[Route("api/v{version:apiVersion}/data-center/route")]
[ApiVersion("1.0")]
public class RouteController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var payload = new { module = "Data Center/Route", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(payload));
    }
}

using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.Factory;

[Route("api/v{version:apiVersion}/data-center/factory")]
[ApiVersion("1.0")]
public class FactoryController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var payload = new { module = "Data Center/Factory", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(payload));
    }
}

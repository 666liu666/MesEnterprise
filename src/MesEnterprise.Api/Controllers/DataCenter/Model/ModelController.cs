using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.Model;

[Route("api/v{version:apiVersion}/data-center/model")]
[ApiVersion("1.0")]
public class ModelController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var payload = new { module = "Data Center/Model", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(payload));
    }
}

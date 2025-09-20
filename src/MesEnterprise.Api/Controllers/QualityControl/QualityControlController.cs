using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.QualityControl;

[Route("api/v{version:apiVersion}/quality-control")]
[ApiVersion("1.0")]
public class QualityControlController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var data = new { module = "QualityControl", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

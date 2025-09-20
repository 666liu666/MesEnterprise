using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.Defect;

[Route("api/v{version:apiVersion}/data-center/defect")]
[ApiVersion("1.0")]
public class DefectController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var payload = new { module = "Data Center/Defect", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(payload));
    }
}

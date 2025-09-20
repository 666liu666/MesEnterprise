using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.DataCenter.KpsnRule;

[Route("api/v{version:apiVersion}/data-center/kpsn-rule")]
[ApiVersion("1.0")]
public class KpsnRuleController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var payload = new { module = "Data Center/KPSN Rule", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(payload));
    }
}

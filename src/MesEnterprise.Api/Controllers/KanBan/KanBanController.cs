using System;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.KanBan;

[Route("api/v{version:apiVersion}/kan-ban")]
[ApiVersion("1.0")]
public class KanBanController : ApiControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var data = new { module = "KanBan", healthy = true, timestamp = DateTimeOffset.UtcNow };
        return Ok(ApiResponse<object>.Ok(data));
    }
}

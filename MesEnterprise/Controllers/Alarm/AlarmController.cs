using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Alarm;

[Authorize]
[Route("api/alarm")]
[ApiExplorerSettings(GroupName = "v1")]
public class AlarmController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Alarm", timestamp = DateTimeOffset.UtcNow });
}

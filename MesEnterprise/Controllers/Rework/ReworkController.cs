using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Rework;

[Authorize]
[Route("api/rework")]
[ApiExplorerSettings(GroupName = "v1")]
public class ReworkController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Rework", timestamp = DateTimeOffset.UtcNow });
}

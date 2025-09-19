using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.ASSY;

[Authorize]
[Route("api/assy")]
[ApiExplorerSettings(GroupName = "v1")]
public class ASSYController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "ASSY", timestamp = DateTimeOffset.UtcNow });
}

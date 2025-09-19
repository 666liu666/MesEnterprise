using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.PCBA;

[Authorize]
[Route("api/pcba")]
[ApiExplorerSettings(GroupName = "v1")]
public class PCBAController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "PCBA", timestamp = DateTimeOffset.UtcNow });
}

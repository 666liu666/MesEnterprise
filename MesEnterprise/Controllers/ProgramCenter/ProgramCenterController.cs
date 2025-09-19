using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.ProgramCenter;

[Authorize]
[Route("api/program-center")]
[ApiExplorerSettings(GroupName = "v1")]
public class ProgramCenterController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Program Center", timestamp = DateTimeOffset.UtcNow });
}

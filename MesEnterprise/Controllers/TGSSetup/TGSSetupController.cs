using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.TGSSetup;

[Authorize]
[Route("api/tgs-setup")]
[ApiExplorerSettings(GroupName = "v1")]
public class TGSSetupController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "TGS Setup", timestamp = DateTimeOffset.UtcNow });
}

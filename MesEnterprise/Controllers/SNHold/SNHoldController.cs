using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.SNHold;

[Authorize]
[Route("api/sn-hold")]
[ApiExplorerSettings(GroupName = "v1")]
public class SNHoldController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "SNHold", timestamp = DateTimeOffset.UtcNow });
}

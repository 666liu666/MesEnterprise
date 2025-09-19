using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.IPQC;

[Authorize]
[Route("api/ipqc")]
[ApiExplorerSettings(GroupName = "v1")]
public class IPQCController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "IPQC", timestamp = DateTimeOffset.UtcNow });
}

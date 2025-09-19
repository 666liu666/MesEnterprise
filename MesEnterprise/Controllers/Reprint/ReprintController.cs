using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Reprint;

[Authorize]
[Route("api/reprint")]
[ApiExplorerSettings(GroupName = "v1")]
public class ReprintController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Reprint", timestamp = DateTimeOffset.UtcNow });
}

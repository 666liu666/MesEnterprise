using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.KIT;

[Authorize]
[Route("api/kit")]
[ApiExplorerSettings(GroupName = "v1")]
public class KITController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "KIT", timestamp = DateTimeOffset.UtcNow });
}

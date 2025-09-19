using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.ATX;

[Authorize]
[Route("api/atx")]
[ApiExplorerSettings(GroupName = "v1")]
public class ATXController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "ATX", timestamp = DateTimeOffset.UtcNow });
}

using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.MoManager;

[Authorize]
[Route("api/mo-manager")]
[ApiExplorerSettings(GroupName = "v1")]
public class MoManagerController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "MoManager", timestamp = DateTimeOffset.UtcNow });
}

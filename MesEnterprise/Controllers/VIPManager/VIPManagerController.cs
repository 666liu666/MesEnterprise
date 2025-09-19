using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.VIPManager;

[Authorize]
[Route("api/vip-manager")]
[ApiExplorerSettings(GroupName = "v1")]
public class VIPManagerController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "VIPManager", timestamp = DateTimeOffset.UtcNow });
}

using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.MSDManager;

[Authorize]
[Route("api/msd-manager")]
[ApiExplorerSettings(GroupName = "v1")]
public class MSDManagerController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "MSD Manager", timestamp = DateTimeOffset.UtcNow });
}

using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Repair;

[Authorize]
[Route("api/repair")]
[ApiExplorerSettings(GroupName = "v1")]
public class RepairController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Repair", timestamp = DateTimeOffset.UtcNow });
}

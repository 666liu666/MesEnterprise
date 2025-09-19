using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.ToolingManager;

[Authorize]
[Route("api/tooling-manager")]
[ApiExplorerSettings(GroupName = "v1")]
public class ToolingManagerController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Tooling Manager", timestamp = DateTimeOffset.UtcNow });
}

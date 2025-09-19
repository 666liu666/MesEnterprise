using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Tool;

[Authorize]
[Route("api/tool")]
[ApiExplorerSettings(GroupName = "v1")]
public class ToolController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Tool", timestamp = DateTimeOffset.UtcNow });
}

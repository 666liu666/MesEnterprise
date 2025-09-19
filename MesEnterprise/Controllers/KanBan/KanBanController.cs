using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.KanBan;

[Authorize]
[Route("api/kanban")]
[ApiExplorerSettings(GroupName = "v1")]
public class KanBanController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "KanBan", timestamp = DateTimeOffset.UtcNow });
}

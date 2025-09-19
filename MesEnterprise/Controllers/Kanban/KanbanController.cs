using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Kanban;

[Authorize]
[Route("api/kanban-secondary")]
[ApiExplorerSettings(GroupName = "v1")]
public class KanbanController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Kanban", timestamp = DateTimeOffset.UtcNow });
}

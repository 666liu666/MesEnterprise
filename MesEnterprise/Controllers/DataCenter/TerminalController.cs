using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/terminal")]
[ApiExplorerSettings(GroupName = "v1")]
public class TerminalController : DataCenterControllerBase
{
    public TerminalController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:terminal:ping", "Data Center - Terminal");
}

using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/shift")]
[ApiExplorerSettings(GroupName = "v1")]
public class ShiftController : DataCenterControllerBase
{
    public ShiftController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:shift:ping", "Data Center - Shift");
}

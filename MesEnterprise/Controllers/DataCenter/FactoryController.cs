using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/factory")]
[ApiExplorerSettings(GroupName = "v1")]
public class FactoryController : DataCenterControllerBase
{
    public FactoryController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:factory:ping", "Data Center - Factory");
}

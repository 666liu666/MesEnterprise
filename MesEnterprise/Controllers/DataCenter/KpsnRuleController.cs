using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/kpsn-rule")]
[ApiExplorerSettings(GroupName = "v1")]
public class KpsnRuleController : DataCenterControllerBase
{
    public KpsnRuleController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:kpsn-rule:ping", "Data Center - KpsnRule");
}

using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/part-master")]
[ApiExplorerSettings(GroupName = "v1")]
public class PartMasterController : DataCenterControllerBase
{
    public PartMasterController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:part-master:ping", "Data Center - PartMaster");
}

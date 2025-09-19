using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/defect")]
[ApiExplorerSettings(GroupName = "v1")]
public class DefectController : DataCenterControllerBase
{
    public DefectController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:defect:ping", "Data Center - Defect");
}

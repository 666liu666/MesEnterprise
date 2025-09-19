using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/vendor")]
[ApiExplorerSettings(GroupName = "v1")]
public class VendorController : DataCenterControllerBase
{
    public VendorController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:vendor:ping", "Data Center - Vendor");
}

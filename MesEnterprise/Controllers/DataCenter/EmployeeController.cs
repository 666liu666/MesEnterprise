using MesEnterprise.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

[Authorize]
[Route("api/data-center/employee")]
[ApiExplorerSettings(GroupName = "v1")]
public class EmployeeController : DataCenterControllerBase
{
    public EmployeeController(ICacheService cacheService) : base(cacheService)
    {
    }

    [HttpGet("ping")]
    public Task<IActionResult> Ping() => CachedPingAsync("data-center:employee:ping", "Data Center - Employee");
}

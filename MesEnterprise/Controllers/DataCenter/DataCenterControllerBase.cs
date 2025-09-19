using MesEnterprise.Cache;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DataCenter;

public abstract class DataCenterControllerBase : MesEnterprise.Controllers.MesControllerBase
{
    protected DataCenterControllerBase(ICacheService cacheService)
    {
        CacheService = cacheService;
    }

    protected ICacheService CacheService { get; }

    protected async Task<IActionResult> CachedPingAsync(string cacheKey, string module)
    {
        var payload = await CacheService.GetAsync(cacheKey, () => Task.FromResult(new
        {
            module,
            timestamp = DateTimeOffset.UtcNow
        }));

        return Success(payload);
    }
}

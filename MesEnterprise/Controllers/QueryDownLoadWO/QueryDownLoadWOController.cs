using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.QueryDownLoadWO;

[Authorize]
[Route("api/query-download-wo")]
[ApiExplorerSettings(GroupName = "v1")]
public class QueryDownLoadWOController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "QueryDownLoadWO", timestamp = DateTimeOffset.UtcNow });
}

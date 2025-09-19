using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Query;

[Authorize]
[Route("api/query")]
[ApiExplorerSettings(GroupName = "v1")]
public class QueryController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Query", timestamp = DateTimeOffset.UtcNow });
}

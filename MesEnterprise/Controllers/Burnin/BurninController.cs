using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Burnin;

[Authorize]
[Route("api/burnin")]
[ApiExplorerSettings(GroupName = "v1")]
public class BurninController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Burnin", timestamp = DateTimeOffset.UtcNow });
}

using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.DIP;

[Authorize]
[Route("api/dip")]
[ApiExplorerSettings(GroupName = "v1")]
public class DIPController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "DIP", timestamp = DateTimeOffset.UtcNow });
}

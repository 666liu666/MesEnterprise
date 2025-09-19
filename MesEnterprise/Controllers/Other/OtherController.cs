using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Other;

[Authorize]
[Route("api/other")]
[ApiExplorerSettings(GroupName = "v1")]
public class OtherController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Other", timestamp = DateTimeOffset.UtcNow });
}

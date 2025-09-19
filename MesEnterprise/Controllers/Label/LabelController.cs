using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Label;

[Authorize]
[Route("api/label")]
[ApiExplorerSettings(GroupName = "v1")]
public class LabelController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Label", timestamp = DateTimeOffset.UtcNow });
}

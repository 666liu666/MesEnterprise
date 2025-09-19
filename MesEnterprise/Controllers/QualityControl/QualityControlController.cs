using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.QualityControl;

[Authorize]
[Route("api/quality-control")]
[ApiExplorerSettings(GroupName = "v1")]
public class QualityControlController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Quality Control", timestamp = DateTimeOffset.UtcNow });
}

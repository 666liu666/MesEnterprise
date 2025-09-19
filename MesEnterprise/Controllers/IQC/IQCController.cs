using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.IQC;

[Authorize]
[Route("api/iqc")]
[ApiExplorerSettings(GroupName = "v1")]
public class IQCController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "IQC", timestamp = DateTimeOffset.UtcNow });
}

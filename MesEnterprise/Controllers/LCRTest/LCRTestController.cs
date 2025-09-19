using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.LCRTest;

[Authorize]
[Route("api/lcr-test")]
[ApiExplorerSettings(GroupName = "v1")]
public class LCRTestController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "LCRTest", timestamp = DateTimeOffset.UtcNow });
}

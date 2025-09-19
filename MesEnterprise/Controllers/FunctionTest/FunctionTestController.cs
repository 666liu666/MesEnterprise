using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.FunctionTest;

[Authorize]
[Route("api/function-test")]
[ApiExplorerSettings(GroupName = "v1")]
public class FunctionTestController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Function Test", timestamp = DateTimeOffset.UtcNow });
}

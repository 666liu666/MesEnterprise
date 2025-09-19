using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.TESTLYNN;

[Authorize]
[Route("api/testlynn")]
[ApiExplorerSettings(GroupName = "v1")]
public class TESTLYNNController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "TESTLYNN", timestamp = DateTimeOffset.UtcNow });
}

using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.SMT;

[Authorize]
[Route("api/smt")]
[ApiExplorerSettings(GroupName = "v1")]
public class SMTController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "SMT", timestamp = DateTimeOffset.UtcNow });
}

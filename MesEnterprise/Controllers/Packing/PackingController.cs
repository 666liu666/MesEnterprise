using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Packing;

[Authorize]
[Route("api/packing")]
[ApiExplorerSettings(GroupName = "v1")]
public class PackingController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Packing", timestamp = DateTimeOffset.UtcNow });
}

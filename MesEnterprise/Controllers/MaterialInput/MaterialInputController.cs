using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.MaterialInput;

[Authorize]
[Route("api/material-input")]
[ApiExplorerSettings(GroupName = "v1")]
public class MaterialInputController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Material Input", timestamp = DateTimeOffset.UtcNow });
}

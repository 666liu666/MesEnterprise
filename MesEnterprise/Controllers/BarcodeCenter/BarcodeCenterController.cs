using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.BarcodeCenter;

[Authorize]
[Route("api/barcode-center")]
[ApiExplorerSettings(GroupName = "v1")]
public class BarcodeCenterController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Barcode Center", timestamp = DateTimeOffset.UtcNow });
}

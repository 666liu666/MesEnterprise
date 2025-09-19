using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Shipping;

[Authorize]
[Route("api/shipping")]
[ApiExplorerSettings(GroupName = "v1")]
public class ShippingController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Shipping", timestamp = DateTimeOffset.UtcNow });
}

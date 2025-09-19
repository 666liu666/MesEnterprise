using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.Inventory;

[Authorize]
[Route("api/inventory")]
[ApiExplorerSettings(GroupName = "v1")]
public class InventoryController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Inventory", timestamp = DateTimeOffset.UtcNow });
}

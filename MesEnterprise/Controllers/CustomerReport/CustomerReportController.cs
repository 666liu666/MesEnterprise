using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.CustomerReport;

[Authorize]
[Route("api/customer-report")]
[ApiExplorerSettings(GroupName = "v1")]
public class CustomerReportController : MesEnterprise.Controllers.MesControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "Customer Report", timestamp = DateTimeOffset.UtcNow });
}

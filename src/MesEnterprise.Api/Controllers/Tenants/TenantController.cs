using MediatR;
using MesEnterprise.Api.Controllers;
using MesEnterprise.Application.Features.Tenants;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Api.Controllers.Tenants;

[Route("api/v{version:apiVersion}/tenants")]
[ApiVersion("1.0")]
public class TenantController : ApiControllerBase
{
    [HttpPost("switch/{tenantId:guid}")]
    public async Task<IActionResult> SwitchTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new SwitchTenantCommand(tenantId), cancellationToken);
        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}

using MesEnterprise.Application.Services;
using MesEnterprise.Auth;
using MesEnterprise.Domain;
using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesEnterprise.Controllers.WOManager;

[Authorize]
[Route("api/wo-manager")]
[ApiExplorerSettings(GroupName = "v1")]
public class WOManagerController : MesEnterprise.Controllers.MesControllerBase
{
    private readonly IWorkOrderService _workOrderService;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public WOManagerController(IWorkOrderService workOrderService, ITenantContextAccessor tenantContextAccessor)
    {
        _workOrderService = workOrderService;
        _tenantContextAccessor = tenantContextAccessor;
    }

    [HttpGet("ping")]
    public IActionResult Ping() => Success(new { module = "W/O Manager", timestamp = DateTimeOffset.UtcNow });

    [HttpGet("active")]
    [PermissionAuthorize("workorder:read")]
    public async Task<IActionResult> GetActiveWorkOrders(CancellationToken cancellationToken)
    {
        var workOrders = await _workOrderService.GetActiveAsync(cancellationToken);
        return Success(new PagedResult<WorkOrder>(workOrders, workOrders.Count, 1, workOrders.Count));
    }

    [HttpPost]
    [PermissionAuthorize("workorder:create")]
    public async Task<IActionResult> CreateWorkOrder([FromBody] CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContextAccessor.Current?.TenantId;
        if (tenantId is null)
        {
            return Failure("Tenant context not resolved", "ERR_TENANT", StatusCodes.Status400BadRequest);
        }

        var workOrder = new WorkOrder
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Product = request.Product,
            Quantity = request.Quantity,
            Status = "Created",
            TenantId = tenantId.Value
        };

        var created = await _workOrderService.CreateAsync(workOrder, cancellationToken);
        return Success(created);
    }
}

public sealed record CreateWorkOrderRequest(string Code, string Product, int Quantity);

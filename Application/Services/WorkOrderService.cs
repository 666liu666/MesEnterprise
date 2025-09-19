using Microsoft.EntityFrameworkCore;
using MesEnterprise.Data;
using MesEnterprise.Domain;

namespace MesEnterprise.Application.Services;

public interface IWorkOrderService
{
    Task<WorkOrder> CreateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WorkOrder>> GetActiveAsync(CancellationToken cancellationToken = default);
}

public sealed class WorkOrderService : IWorkOrderService
{
    private readonly MesDbContext _dbContext;

    public WorkOrderService(MesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkOrder> CreateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkOrders.AddAsync(workOrder, cancellationToken);
        await _dbContext.WorkOrderHistory.AddAsync(new WorkOrderHistory
        {
            WorkOrderId = workOrder.Id,
            Operation = "Create",
            Details = $"WO {workOrder.Code} created with quantity {workOrder.Quantity}",
            TenantId = workOrder.TenantId
        }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return workOrder;
    }

    public async Task<IReadOnlyCollection<WorkOrder>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.WorkOrders
            .Where(order => order.Status != "Closed")
            .OrderBy(order => order.Code)
            .ToListAsync(cancellationToken);

        return result;
    }
}

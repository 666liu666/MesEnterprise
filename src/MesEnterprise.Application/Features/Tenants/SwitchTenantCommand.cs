using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Shared.Responses;

namespace MesEnterprise.Application.Features.Tenants;

public record SwitchTenantCommand(Guid TenantId) : IRequest<ApiResponse<bool>>;

public class SwitchTenantCommandHandler : IRequestHandler<SwitchTenantCommand, ApiResponse<bool>>
{
    private readonly ITenantResolver _tenantResolver;

    public SwitchTenantCommandHandler(ITenantResolver tenantResolver)
    {
        _tenantResolver = tenantResolver;
    }

    public async Task<ApiResponse<bool>> Handle(SwitchTenantCommand request, CancellationToken cancellationToken)
    {
        var result = await _tenantResolver.TrySwitchTenantAsync(request.TenantId, cancellationToken);
        return result ? ApiResponse<bool>.Ok(true) : ApiResponse<bool>.Fail("Tenant not found");
    }
}

public interface ITenantResolver
{
    Task<bool> TrySwitchTenantAsync(Guid tenantId, CancellationToken cancellationToken);
}

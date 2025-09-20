using System;
using System.Linq.Expressions;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.Infrastructure.Multitenancy;

public static class TenantModelBuilderExtensions
{
    public static void ApplyGlobalTenantFilter(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        var tenantId = tenantContext.CurrentTenant?.Id;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(TenantModelBuilderExtensions)
                .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.MakeGenericMethod(entityType.ClrType);

            method?.Invoke(null, new object?[] { modelBuilder, tenantId });
        }
    }

    private static void SetTenantFilter<TEntity>(ModelBuilder builder, Guid? tenantId) where TEntity : BaseEntity
    {
        Expression<Func<TEntity, bool>> filter = tenant => tenantId == null || tenant.TenantId == tenantId;
        builder.Entity<TEntity>().HasQueryFilter(filter);
    }
}

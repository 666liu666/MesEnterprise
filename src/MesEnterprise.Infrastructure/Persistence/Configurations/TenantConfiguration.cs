using MesEnterprise.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MesEnterprise.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("SYS_TENANTS");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(128).IsRequired();
        builder.Property(t => t.Code).HasMaxLength(32);
        builder.Property(t => t.ConnectionString).HasMaxLength(512);
    }
}

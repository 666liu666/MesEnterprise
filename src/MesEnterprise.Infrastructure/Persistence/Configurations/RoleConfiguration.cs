using MesEnterprise.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MesEnterprise.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("SYS_ROLES");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(128).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(256);
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("SYS_ROLE_PERMISSIONS");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("SYS_USER_ROLES");
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("SYS_PERMISSIONS");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Category).HasMaxLength(64);
    }
}

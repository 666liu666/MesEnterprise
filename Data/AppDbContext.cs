using MesEnterprise.Domain;
using MesEnterprise.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<SysFactory> Factories => Set<SysFactory>();
    public DbSet<SysHtFactory> HtFactories => Set<SysHtFactory>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Machine> Machines => Set<Machine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SysFactory>(entity =>
        {
            entity.ToTable("SYS_FACTORY", "SAJET");
            entity.HasKey(e => e.FactoryId);

            entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
            entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE").HasMaxLength(10);
            entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME").HasMaxLength(25);
            entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC").HasMaxLength(50);
            entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
            entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
            entity.Property(e => e.Enabled).HasColumnName("ENABLED").HasMaxLength(1);
        });

        modelBuilder.Entity<SysHtFactory>(entity =>
        {
            entity.ToTable("SYS_HT_FACTORY", "SAJET");
            entity.HasKey(e => new { e.FactoryId, e.UpdateTime });

            entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
            entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE").HasMaxLength(10);
            entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME").HasMaxLength(25);
            entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC").HasMaxLength(50);
            entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
            entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
            entity.Property(e => e.Enabled).HasColumnName("ENABLED").HasMaxLength(1);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(128);
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId });
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId);
            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId);
        });
    }
}

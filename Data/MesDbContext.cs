using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MesEnterprise.Domain;
using MesEnterprise.Models;
using MesEnterprise.Shared;

namespace MesEnterprise.Data;

public class MesDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly ITenantContextAccessor _tenantAccessor;

    public MesDbContext(DbContextOptions<MesDbContext> options, ITenantContextAccessor tenantAccessor)
        : base(options)
    {
        _tenantAccessor = tenantAccessor;
    }

    public DbSet<SysFactory> Factories => Set<SysFactory>();
    public DbSet<SysHtFactory> HtFactories => Set<SysHtFactory>();

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderHistory> WorkOrderHistory => Set<WorkOrderHistory>();
    public DbSet<Machine> Machines => Set<Machine>();

    private Guid? CurrentTenantId => _tenantAccessor.Current?.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("SAJET");

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("SYS_USERS");
            entity.Property(x => x.DisplayName).HasColumnName("DISPLAY_NAME").HasMaxLength(100);
            entity.Property(x => x.TenantId).HasColumnName("TENANT_ID");
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("SYS_ROLES");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("SYS_ROLE_PERMISSION");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);
            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("SYS_PERMISSION");
            entity.Property(x => x.Code).HasMaxLength(100);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("SYS_TENANT");
            entity.Property(x => x.Identifier).HasMaxLength(50);
            entity.Property(x => x.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("SYS_AUDIT_LOG");
            entity.Property(x => x.Path).HasMaxLength(400);
            entity.Property(x => x.Method).HasMaxLength(20);
            entity.Property(x => x.UserId).HasMaxLength(36);
            entity.Property(x => x.Exception).HasColumnType("CLOB");
            entity.Property(x => x.RequestBody).HasColumnType("CLOB");
            entity.Property(x => x.ResponseBody).HasColumnType("CLOB");
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.ToTable("MES_WORK_ORDER");
            entity.Property(x => x.Code).HasMaxLength(100);
            entity.Property(x => x.Product).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(50);
            entity.Property(x => x.TenantId).HasColumnName("TENANT_ID");
        });

        modelBuilder.Entity<WorkOrderHistory>(entity =>
        {
            entity.ToTable("MES_WORK_ORDER_HIS");
            entity.Property(x => x.Operation).HasMaxLength(100);
            entity.Property(x => x.TenantId).HasColumnName("TENANT_ID");
        });

        modelBuilder.Entity<Machine>(entity =>
        {
            entity.ToTable("MES_MACHINE");
            entity.Property(x => x.Code).HasMaxLength(100);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(50);
            entity.Property(x => x.TenantId).HasColumnName("TENANT_ID");
        });

        modelBuilder.Entity<SysFactory>(entity =>
        {
            entity.ToTable("SYS_FACTORY");
            entity.HasKey(e => e.FactoryId);

            entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
            entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE");
            entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME");
            entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC");
            entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
            entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
            entity.Property(e => e.Enabled).HasColumnName("ENABLED");
        });

        modelBuilder.Entity<SysHtFactory>(entity =>
        {
            entity.ToTable("SYS_HT_FACTORY");
            entity.HasKey(e => e.FactoryId);

            entity.Property(e => e.FactoryId).HasColumnName("FACTORY_ID");
            entity.Property(e => e.FactoryCode).HasColumnName("FACTORY_CODE");
            entity.Property(e => e.FactoryName).HasColumnName("FACTORY_NAME");
            entity.Property(e => e.FactoryDesc).HasColumnName("FACTORY_DESC");
            entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
            entity.Property(e => e.UpdateTime).HasColumnName("UPDATE_TIME");
            entity.Property(e => e.Enabled).HasColumnName("ENABLED");
        });

        ApplyTenantFilters(modelBuilder);
    }

    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>().HasQueryFilter(entity => CurrentTenantId == null || entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<WorkOrderHistory>().HasQueryFilter(entity => CurrentTenantId == null || entity.TenantId == CurrentTenantId);
        modelBuilder.Entity<Machine>().HasQueryFilter(entity => CurrentTenantId == null || entity.TenantId == CurrentTenantId);
    }
}

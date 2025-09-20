using Microsoft.EntityFrameworkCore;
using MesEnterprise.Domain.Audit;
using MesEnterprise.Domain.DataCenter;
using MesEnterprise.Domain.Identity;
using MesEnterprise.Domain.Inventory;
using MesEnterprise.Domain.Manufacturing;
using MesEnterprise.Domain.Quality;
using MesEnterprise.Domain.Tenants;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Infrastructure.Multitenancy;

namespace MesEnterprise.Infrastructure.Persistence;

public class MesDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITenantContext _tenantContext;

    public MesDbContext(DbContextOptions<MesDbContext> options, ICurrentUserService currentUserService, IDateTimeProvider dateTimeProvider, ITenantContext tenantContext)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _tenantContext = tenantContext;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserClaim> UserClaims => Set<UserClaim>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ProcessDefinition> Processes => Set<ProcessDefinition>();
    public DbSet<RouteDefinition> Routes => Set<RouteDefinition>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<ModelDefinition> Models => Set<ModelDefinition>();
    public DbSet<ShiftDefinition> Shifts => Set<ShiftDefinition>();
    public DbSet<PartMaster> Parts => Set<PartMaster>();
    public DbSet<Factory> Factories => Set<Factory>();
    public DbSet<BillOfMaterial> BillsOfMaterial => Set<BillOfMaterial>();
    public DbSet<KpsnRule> KpsnRules => Set<KpsnRule>();
    public DbSet<Terminal> Terminals => Set<Terminal>();
    public DbSet<DefectCode> Defects => Set<DefectCode>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<QualityRecord> QualityRecords => Set<QualityRecord>();
    public DbSet<InventoryItem> Inventory => Set<InventoryItem>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MesDbContext).Assembly);
        modelBuilder.ApplyGlobalTenantFilter(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is MesEnterprise.Domain.Common.AuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = _dateTimeProvider.UtcNow;
                        auditable.CreatedBy = _currentUserService.UserName;
                        auditable.TenantId = _tenantContext.CurrentTenant?.Id ?? Guid.Empty;
                        break;
                    case EntityState.Modified:
                        auditable.LastModifiedAt = _dateTimeProvider.UtcNow;
                        auditable.LastModifiedBy = _currentUserService.UserName;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

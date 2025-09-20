using System;
using System.Threading;
using System.Threading.Tasks;
using MesEnterprise.Domain.Identity;
using MesEnterprise.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MesEnterprise.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");
        var context = scope.ServiceProvider.GetRequiredService<MesDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        if (!await context.Users.AnyAsync(cancellationToken))
        {
            var adminRole = new Role { Name = "Administrator", Description = "System administrator" };
            var adminUser = new User
            {
                UserName = "admin",
                DisplayName = "System Admin",
                Email = "admin@mes.local",
                PasswordHash = IdentityService.HashPassword("Mes@123456"),
                TenantId = Guid.Empty
            };

            var permission = new Permission { Code = "permissions.admin", Name = "Administrator" };
            adminRole.Permissions.Add(new RolePermission { Role = adminRole, Permission = permission });
            adminUser.Roles.Add(new UserRole { User = adminUser, Role = adminRole });

            await context.Roles.AddAsync(adminRole, cancellationToken);
            await context.Permissions.AddAsync(permission, cancellationToken);
            await context.Users.AddAsync(adminUser, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded default administrator user");
        }
    }
}

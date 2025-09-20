using MesEnterprise.Domain.Identity;

namespace MesEnterprise.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<User?> GetUserAsync(string userName, CancellationToken cancellationToken);
    Task<bool> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken);
}

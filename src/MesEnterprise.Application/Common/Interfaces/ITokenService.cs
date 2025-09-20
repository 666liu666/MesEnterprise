using MesEnterprise.Domain.Identity;

namespace MesEnterprise.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user, IEnumerable<string> roles, IEnumerable<string> permissions, CancellationToken cancellationToken);
    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
}

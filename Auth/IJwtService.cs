using MesEnterprise.Domain;

namespace MesEnterprise.Auth;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
}

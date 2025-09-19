using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MesEnterprise.Domain;

namespace MesEnterprise.Auth;

public class JwtService : IJwtService
{
    private readonly JwtOptions _options;

    public JwtService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(_options.Key))
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.DisplayName))
        {
            claims.Add(new Claim("displayName", user.DisplayName));
        }

        claims.AddRange(roles.Distinct().Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Distinct().Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(_options.ExpiryHours <= 0 ? 8 : _options.ExpiryHours);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

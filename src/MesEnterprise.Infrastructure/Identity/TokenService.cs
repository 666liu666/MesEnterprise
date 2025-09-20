using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MesEnterprise.Application.Common.Interfaces;
using MesEnterprise.Domain.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MesEnterprise.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public Task<string> GenerateTokenAsync(User user, IEnumerable<string> roles, IEnumerable<string> permissions, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new("displayName", user.DisplayName),
            new("tenant", user.TenantId.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenLifetimeMinutes),
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var bytes = Guid.NewGuid().ToByteArray();
        return Task.FromResult(Convert.ToBase64String(bytes));
    }
}

public class JwtSettings
{
    public string Issuer { get; set; } = "MesEnterprise";
    public string Audience { get; set; } = "MesEnterprise";
    public string SigningKey { get; set; } = "ChangeThisSigningKey";
    public int AccessTokenLifetimeMinutes { get; set; } = 120;
}

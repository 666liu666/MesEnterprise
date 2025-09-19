using Auth;
using MesEnterprise.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MesEnterprise.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _opts;
        public JwtService(IOptions<JwtOptions> opts) { _opts = opts.Value; }
        public string GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new Claim("name", user.UserName ?? "") };
            foreach (var r in roles.Distinct()) claims.Add(new Claim(ClaimTypes.Role, r));
            foreach (var p in permissions.Distinct()) claims.Add(new Claim("permissions", p));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_opts.Issuer, _opts.Audience, claims, expires: DateTime.UtcNow.AddHours(_opts.ExpiryHours), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MesEnterprise.Domain;
using MesEnterprise.Auth;
using MesEnterprise.Data;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly SignInManager<ApplicationUser> _sm;
        private readonly RoleManager<ApplicationRole> _rm;
        private readonly IJwtService _jwt;
        private readonly AppDbContext _db;
        public AuthController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, RoleManager<ApplicationRole> rm, IJwtService jwt, AppDbContext db)
        { _um = um; _sm = sm; _rm = rm; _jwt = jwt; _db = db; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var u = new ApplicationUser { UserName = dto.Username, DisplayName = dto.DisplayName };
            var r = await _um.CreateAsync(u, dto.Password);
            if (!r.Succeeded) return BadRequest(r.Errors);
            if (!string.IsNullOrEmpty(dto.Role))
            {
                if (!await _rm.RoleExistsAsync(dto.Role)) await _rm.CreateAsync(new ApplicationRole { Name = dto.Role });
                await _um.AddToRoleAsync(u, dto.Role);
            }
            return Ok(new { u.Id, u.UserName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _um.FindByNameAsync(dto.Username);
            if (user == null) return Unauthorized();
            var signin = await _sm.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!signin.Succeeded) return Unauthorized();
            var roles = await _um.GetRolesAsync(user);
            var perms = await _db.RolePermissions.Where(rp => roles.Contains(rp.Role!.Name!)).Select(rp => rp.Permission!.Code).Distinct().ToListAsync();
            var token = _jwt.GenerateToken(user, roles, perms);
            return Ok(new { token, expiresInHours = 8 });
        }
    }

    public class RegisterDto { public string Username { get; set; } = null!; public string Password { get; set; } = null!; public string? DisplayName { get; set; } public string? Role { get; set; } }
    public class LoginDto { public string Username { get; set; } = null!; public string Password { get; set; } = null!; }
}


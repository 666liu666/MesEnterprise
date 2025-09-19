using MesEnterprise.Auth;
using MesEnterprise.Data;
using MesEnterprise.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.ServiceDefaults;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly MesDbContext _dbContext;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtService jwtService,
        MesDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Username, DisplayName = dto.DisplayName, TenantId = dto.TenantId };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = dto.Role });
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        return Ok(new { user.Id, user.UserName, user.DisplayName });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.Username);
        if (user == null)
        {
            return Unauthorized();
        }

        var signin = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!signin.Succeeded)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _dbContext.RolePermissions
            .Where(rp => roles.Contains(rp.Role!.Name!))
            .Select(rp => rp.Permission!.Code)
            .Distinct()
            .ToListAsync();

        var token = _jwtService.GenerateToken(user, roles, permissions);
        return Ok(new { token, expiresInHours = 8 });
    }
}

public sealed record RegisterDto(string Username, string Password, Guid TenantId, string? DisplayName, string? Role);
public sealed record LoginDto(string Username, string Password);

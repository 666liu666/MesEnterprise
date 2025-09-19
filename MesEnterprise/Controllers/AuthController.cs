using MesEnterprise.Auth;
using MesEnterprise.Data;
using MesEnterprise.Domain;
using MesEnterprise.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MesEnterprise.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthController : MesControllerBase
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
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
            DisplayName = request.DisplayName,
            TenantId = request.TenantId
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Failure(string.Join(";", result.Errors.Select(e => e.Description)), "ERR_REGISTER");
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = request.Role });
            }

            await _userManager.AddToRoleAsync(user, request.Role);
        }

        return Success(new { user.Id, user.UserName, user.DisplayName });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);
        if (user is null)
        {
            return Failure("Invalid credentials", "ERR_LOGIN", StatusCodes.Status401Unauthorized);
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded)
        {
            return Failure("Invalid credentials", "ERR_LOGIN", StatusCodes.Status401Unauthorized);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _dbContext.RolePermissions
            .Where(rp => roles.Contains(rp.Role!.Name!))
            .Select(rp => rp.Permission!.Code)
            .Distinct()
            .ToListAsync();

        var token = _jwtService.GenerateToken(user, roles, permissions);
        return Success(new { token, expiresInHours = 8, user = new { user.Id, user.UserName, user.DisplayName } });
    }
}

public sealed record RegisterRequest(string Username, string Password, Guid TenantId, string? DisplayName, string? Role);
public sealed record LoginRequest(string Username, string Password);

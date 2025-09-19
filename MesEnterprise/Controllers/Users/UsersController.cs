using System.Linq;
using ClosedXML.Excel;
using MesEnterprise.Domain;
using MesEnterprise.Infrastructure;
using MesEnterprise.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MesEnterprise.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer<UsersController> _localizer;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer<UsersController> localizer,
        IStringLocalizer<SharedResource> sharedLocalizer)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _localizer = localizer;
        _sharedLocalizer = sharedLocalizer;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserSummaryDto>>>> GetUsers([FromQuery] UserQuery query, CancellationToken cancellationToken)
    {
        var usersQuery = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            usersQuery = usersQuery.Where(u =>
                (u.UserName != null && u.UserName.Contains(keyword)) ||
                (u.Email != null && u.Email.Contains(keyword)) ||
                (u.DisplayName != null && u.DisplayName.Contains(keyword)));
        }

        var totalCount = await usersQuery.CountAsync(cancellationToken);
        var users = await usersQuery
            .OrderBy(u => u.UserName)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var summaries = new List<UserSummaryDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            summaries.Add(new UserSummaryDto(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email,
                user.DisplayName,
                IsEnabled(user),
                roles));
        }

        var result = new PagedResult<UserSummaryDto>(summaries, totalCount, query.PageNumber, query.PageSize);
        return ApiResponse<PagedResult<UserSummaryDto>>.Ok(result, _sharedLocalizer["QuerySuccess"]);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetUser(Guid id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDetailDto>.Fail(_localizer["UserNotFound"]));
        }

        var roles = await _userManager.GetRolesAsync(user);
        var dto = new UserDetailDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email,
            user.DisplayName,
            IsEnabled(user),
            roles,
            user.LockoutEnd);

        return ApiResponse<UserDetailDto>.Ok(dto, _sharedLocalizer["QuerySuccess"]);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> CreateUser([FromBody] UserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            Email = request.Email?.Trim(),
            DisplayName = request.DisplayName?.Trim(),
            LockoutEnabled = true
        };

        var password = string.IsNullOrWhiteSpace(request.Password)
            ? GenerateTemporaryPassword()
            : request.Password!;

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<UserDetailDto>.Fail(_sharedLocalizer["OperationFailed"], result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        }

        await UpdateRolesAsync(user, request.Roles);
        await SetEnabledAsync(user, request.Enabled);

        var roles = await _userManager.GetRolesAsync(user);
        var dto = new UserDetailDto(user.Id, user.UserName ?? string.Empty, user.Email, user.DisplayName, IsEnabled(user), roles, user.LockoutEnd);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<UserDetailDto>.Ok(dto, _localizer["UserCreated"]));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> UpdateUser(Guid id, [FromBody] UserRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound(ApiResponse<UserDetailDto>.Fail(_localizer["UserNotFound"]));
        }

        user.Email = request.Email?.Trim();
        user.DisplayName = request.DisplayName?.Trim();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(ApiResponse<UserDetailDto>.Fail(_sharedLocalizer["OperationFailed"], updateResult.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!passwordResult.Succeeded)
            {
                return BadRequest(ApiResponse<UserDetailDto>.Fail(_sharedLocalizer["OperationFailed"], passwordResult.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
            }
        }

        await UpdateRolesAsync(user, request.Roles);
        await SetEnabledAsync(user, request.Enabled);

        var roles = await _userManager.GetRolesAsync(user);
        var dto = new UserDetailDto(user.Id, user.UserName ?? string.Empty, user.Email, user.DisplayName, IsEnabled(user), roles, user.LockoutEnd);
        return ApiResponse<UserDetailDto>.Ok(dto, _localizer["UserUpdated"]);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound(ApiResponse<object>.Fail(_localizer["UserNotFound"]));
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<object>.Fail(_sharedLocalizer["OperationFailed"], result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray())));
        }

        return ApiResponse<object>.Ok(_localizer["UserDeleted"]);
    }

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<UserImportSummary>>> ImportUsers([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<UserImportSummary>.Fail(_sharedLocalizer["InvalidExcel"]));
        }

        var created = 0;
        var updated = 0;
        var skipped = 0;

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1) ?? Enumerable.Empty<IXLRangeRow>();

        foreach (var row in rows)
        {
            var userName = row.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(userName))
            {
                skipped++;
                continue;
            }

            var displayName = row.Cell(2).GetString();
            var email = row.Cell(3).GetString();
            var password = row.Cell(4).GetString();
            var roles = row.Cell(5).GetString()?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
            var enabledCell = row.Cell(6).GetString();
            var enabled = !string.Equals(enabledCell, "N", StringComparison.OrdinalIgnoreCase);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    LockoutEnabled = true
                };

                var createResult = string.IsNullOrWhiteSpace(password)
                    ? await _userManager.CreateAsync(user, GenerateTemporaryPassword())
                    : await _userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    skipped++;
                    continue;
                }

                await UpdateRolesAsync(user, roles);
                await SetEnabledAsync(user, enabled);
                created++;
            }
            else
            {
                user.DisplayName = string.IsNullOrWhiteSpace(displayName) ? user.DisplayName : displayName;
                user.Email = string.IsNullOrWhiteSpace(email) ? user.Email : email;
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrWhiteSpace(password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, password);
                }

                await UpdateRolesAsync(user, roles);
                await SetEnabledAsync(user, enabled);
                updated++;
            }
        }

        var summary = new UserImportSummary(created, updated, skipped);
        var message = _localizer["UserImportSummary", created, updated, skipped];
        return ApiResponse<UserImportSummary>.Ok(summary, message);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportUsers([FromQuery] UserQuery query, CancellationToken cancellationToken)
    {
        var usersQuery = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            usersQuery = usersQuery.Where(u =>
                (u.UserName != null && u.UserName.Contains(keyword)) ||
                (u.Email != null && u.Email.Contains(keyword)) ||
                (u.DisplayName != null && u.DisplayName.Contains(keyword)));
        }

        var users = await usersQuery.OrderBy(u => u.UserName).ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(_localizer["ExportSheetName"]);
        worksheet.Cell(1, 1).Value = _localizer["UserNameHeader"];
        worksheet.Cell(1, 2).Value = _localizer["DisplayNameHeader"];
        worksheet.Cell(1, 3).Value = _localizer["EmailHeader"];
        worksheet.Cell(1, 4).Value = _localizer["RolesHeader"];
        worksheet.Cell(1, 5).Value = _localizer["EnabledHeader"];

        var rowIndex = 2;
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            worksheet.Cell(rowIndex, 1).Value = user.UserName;
            worksheet.Cell(rowIndex, 2).Value = user.DisplayName;
            worksheet.Cell(rowIndex, 3).Value = user.Email;
            worksheet.Cell(rowIndex, 4).Value = string.Join(",", roles);
            worksheet.Cell(rowIndex, 5).Value = IsEnabled(user) ? "Y" : "N";
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var exportStream = new MemoryStream();
        workbook.SaveAs(exportStream);
        exportStream.Position = 0;

        var fileName = string.Format(_localizer["ExportFileName"], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        return File(exportStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    private static string GenerateTemporaryPassword() => $"Aa1!{Guid.NewGuid():N}";

    private static bool IsEnabled(ApplicationUser user)
    {
        if (!user.LockoutEnabled)
        {
            return true;
        }

        return user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow;
    }

    private async Task UpdateRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var targetRoles = roles.Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var currentRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in targetRoles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        var rolesToRemove = currentRoles.Except(targetRoles, StringComparer.OrdinalIgnoreCase);
        var rolesToAdd = targetRoles.Except(currentRoles, StringComparer.OrdinalIgnoreCase);

        if (rolesToRemove.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        if (rolesToAdd.Any())
        {
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }
    }

    private async Task SetEnabledAsync(ApplicationUser user, bool enabled)
    {
        if (enabled)
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
    }
}

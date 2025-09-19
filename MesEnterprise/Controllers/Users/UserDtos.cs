using System.ComponentModel.DataAnnotations;

namespace MesEnterprise.Controllers.Users;

public class UserQuery
{
    private const int MaxPageSize = 200;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; set; } = 20;

    public string? Keyword { get; set; }
}

public class UserRequest
{
    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? DisplayName { get; set; }

    [StringLength(200)]
    public string? Password { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();

    public bool Enabled { get; set; } = true;
}

public record UserSummaryDto(Guid Id, string UserName, string? Email, string? DisplayName, bool Enabled, IEnumerable<string> Roles);

public record UserDetailDto(Guid Id, string UserName, string? Email, string? DisplayName, bool Enabled, IEnumerable<string> Roles, DateTimeOffset? LockoutEnd);

public record UserImportSummary(int Created, int Updated, int Skipped);

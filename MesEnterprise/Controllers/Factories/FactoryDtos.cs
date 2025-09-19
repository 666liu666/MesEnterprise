using System.ComponentModel.DataAnnotations;

namespace MesEnterprise.Controllers.Factories;

public class FactoryQuery
{
    private const int MaxPageSize = 200;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; set; } = 20;

    public string? Keyword { get; set; }

    public bool? Enabled { get; set; }
}

public class FactoryRequest
{
    [Required]
    [StringLength(10)]
    public string FactoryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(25)]
    public string FactoryName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? FactoryDesc { get; set; }

    public bool Enabled { get; set; } = true;
}

public record FactoryDto(decimal FactoryId, string FactoryCode, string FactoryName, string? FactoryDesc, bool Enabled, DateTime? UpdateTime, decimal? UpdateUserId);

public record FactoryHistoryDto(DateTime? UpdateTime, string FactoryCode, string FactoryName, string? FactoryDesc, bool Enabled, decimal? UpdateUserId);

public record FactoryImportSummary(int Created, int Updated, int Skipped);

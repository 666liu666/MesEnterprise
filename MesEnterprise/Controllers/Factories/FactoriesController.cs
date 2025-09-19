using System.Linq;
using ClosedXML.Excel;
using MesEnterprise.Data;
using MesEnterprise.Infrastructure;
using MesEnterprise.Models;
using MesEnterprise.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MesEnterprise.Controllers.Factories;

[ApiController]
[Route("api/[controller]")]
public class FactoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IStringLocalizer<FactoriesController> _localizer;
    private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
    public FactoriesController(
        AppDbContext db,
        IStringLocalizer<FactoriesController> localizer,
        IStringLocalizer<SharedResource> sharedLocalizer)
    {
        _db = db;
        _localizer = localizer;
        _sharedLocalizer = sharedLocalizer;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FactoryDto>>>> GetFactories([FromQuery] FactoryQuery query, CancellationToken cancellationToken)
    {
        var baseQuery = _db.Factories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            baseQuery = baseQuery.Where(f => f.FactoryCode!.Contains(keyword) || f.FactoryName!.Contains(keyword));
        }

        if (query.Enabled.HasValue)
        {
            baseQuery = baseQuery.Where(f => f.Enabled == ToFlag(query.Enabled.Value));
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var items = await baseQuery
            .OrderByDescending(f => f.UpdateTime ?? DateTime.MinValue)
            .ThenBy(f => f.FactoryCode)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(f => new FactoryDto(
                f.FactoryId,
                f.FactoryCode ?? string.Empty,
                f.FactoryName ?? string.Empty,
                f.FactoryDesc,
                FromFlag(f.Enabled),
                f.UpdateTime,
                f.UpdateUserId))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<FactoryDto>(items, totalCount, query.PageNumber, query.PageSize);
        return ApiResponse<PagedResult<FactoryDto>>.Ok(result, _sharedLocalizer["QuerySuccess"]);
    }

    [HttpGet("{id:decimal}")]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> GetFactory(decimal id, CancellationToken cancellationToken)
    {
        var entity = await _db.Factories.AsNoTracking().FirstOrDefaultAsync(f => f.FactoryId == id, cancellationToken);
        if (entity == null)
        {
            return NotFound(ApiResponse<FactoryDto>.Fail(_localizer["FactoryNotFound"]));
        }

        var dto = new FactoryDto(
            entity.FactoryId,
            entity.FactoryCode ?? string.Empty,
            entity.FactoryName ?? string.Empty,
            entity.FactoryDesc,
            FromFlag(entity.Enabled),
            entity.UpdateTime,
            entity.UpdateUserId);

        return ApiResponse<FactoryDto>.Ok(dto, _sharedLocalizer["QuerySuccess"]);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> CreateFactory([FromBody] FactoryRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var nextId = await GenerateFactoryIdAsync(cancellationToken);

        var entity = new SysFactory
        {
            FactoryId = nextId,
            FactoryCode = request.FactoryCode.Trim(),
            FactoryName = request.FactoryName.Trim(),
            FactoryDesc = request.FactoryDesc?.Trim(),
            Enabled = ToFlag(request.Enabled),
            UpdateTime = now
        };

        _db.Factories.Add(entity);
        _db.HtFactories.Add(CreateHistory(entity));
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new FactoryDto(entity.FactoryId, entity.FactoryCode!, entity.FactoryName!, entity.FactoryDesc, request.Enabled, entity.UpdateTime, entity.UpdateUserId);
        return CreatedAtAction(nameof(GetFactory), new { id = entity.FactoryId }, ApiResponse<FactoryDto>.Ok(dto, _localizer["FactoryCreated"]));
    }

    [HttpPut("{id:decimal}")]
    public async Task<ActionResult<ApiResponse<FactoryDto>>> UpdateFactory(decimal id, [FromBody] FactoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _db.Factories.FirstOrDefaultAsync(f => f.FactoryId == id, cancellationToken);
        if (entity == null)
        {
            return NotFound(ApiResponse<FactoryDto>.Fail(_localizer["FactoryNotFound"]));
        }

        entity.FactoryCode = request.FactoryCode.Trim();
        entity.FactoryName = request.FactoryName.Trim();
        entity.FactoryDesc = request.FactoryDesc?.Trim();
        entity.Enabled = ToFlag(request.Enabled);
        entity.UpdateTime = DateTime.UtcNow;

        _db.HtFactories.Add(CreateHistory(entity));
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new FactoryDto(entity.FactoryId, entity.FactoryCode!, entity.FactoryName!, entity.FactoryDesc, request.Enabled, entity.UpdateTime, entity.UpdateUserId);
        return ApiResponse<FactoryDto>.Ok(dto, _localizer["FactoryUpdated"]);
    }

    [HttpDelete("{id:decimal}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFactory(decimal id, CancellationToken cancellationToken)
    {
        var entity = await _db.Factories.FirstOrDefaultAsync(f => f.FactoryId == id, cancellationToken);
        if (entity == null)
        {
            return NotFound(ApiResponse<object>.Fail(_localizer["FactoryNotFound"]));
        }

        entity.Enabled = ToFlag(false);
        entity.UpdateTime = DateTime.UtcNow;
        _db.HtFactories.Add(CreateHistory(entity));
        await _db.SaveChangesAsync(cancellationToken);

        return ApiResponse<object>.Ok(_localizer["FactoryDeleted"]);
    }

    [HttpGet("{id:decimal}/history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FactoryHistoryDto>>>> GetHistory(decimal id, CancellationToken cancellationToken)
    {
        var history = await _db.HtFactories
            .AsNoTracking()
            .Where(h => h.FactoryId == id)
            .OrderByDescending(h => h.UpdateTime)
            .Select(h => new FactoryHistoryDto(
                h.UpdateTime,
                h.FactoryCode ?? string.Empty,
                h.FactoryName ?? string.Empty,
                h.FactoryDesc,
                FromFlag(h.Enabled),
                h.UpdateUserId))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<FactoryHistoryDto>>.Ok(history, _sharedLocalizer["QuerySuccess"]);
    }

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<FactoryImportSummary>>> ImportFactories([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<FactoryImportSummary>.Fail(_sharedLocalizer["InvalidExcel"]));
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
            var code = row.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                skipped++;
                continue;
            }

            var name = row.Cell(2).GetString().Trim();
            var description = row.Cell(3).GetString();
            var enabledCell = row.Cell(4).GetString();
            var enabled = enabledCell.Equals("Y", StringComparison.OrdinalIgnoreCase) || enabledCell.Equals("1");

            var entity = await _db.Factories.FirstOrDefaultAsync(f => f.FactoryCode == code, cancellationToken);
            if (entity == null)
            {
                var newEntity = new SysFactory
                {
                    FactoryId = await GenerateFactoryIdAsync(cancellationToken),
                    FactoryCode = code,
                    FactoryName = string.IsNullOrWhiteSpace(name) ? code : name,
                    FactoryDesc = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                    Enabled = ToFlag(enabled),
                    UpdateTime = DateTime.UtcNow
                };

                _db.Factories.Add(newEntity);
                _db.HtFactories.Add(CreateHistory(newEntity));
                created++;
            }
            else
            {
                entity.FactoryName = string.IsNullOrWhiteSpace(name) ? entity.FactoryName : name;
                entity.FactoryDesc = string.IsNullOrWhiteSpace(description) ? entity.FactoryDesc : description.Trim();
                entity.Enabled = ToFlag(enabled);
                entity.UpdateTime = DateTime.UtcNow;
                _db.HtFactories.Add(CreateHistory(entity));
                updated++;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        var summary = new FactoryImportSummary(created, updated, skipped);
        var message = _localizer["FactoryImportSummary", created, updated, skipped];
        return ApiResponse<FactoryImportSummary>.Ok(summary, message);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportFactories([FromQuery] FactoryQuery query, CancellationToken cancellationToken)
    {
        var baseQuery = _db.Factories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            baseQuery = baseQuery.Where(f => f.FactoryCode!.Contains(keyword) || f.FactoryName!.Contains(keyword));
        }

        if (query.Enabled.HasValue)
        {
            baseQuery = baseQuery.Where(f => f.Enabled == ToFlag(query.Enabled.Value));
        }

        var factories = await baseQuery
            .OrderBy(f => f.FactoryCode)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(_localizer["ExportSheetName"]);
        worksheet.Cell(1, 1).Value = _localizer["FactoryCodeHeader"];
        worksheet.Cell(1, 2).Value = _localizer["FactoryNameHeader"];
        worksheet.Cell(1, 3).Value = _localizer["FactoryDescHeader"];
        worksheet.Cell(1, 4).Value = _localizer["EnabledHeader"];
        worksheet.Cell(1, 5).Value = _localizer["UpdateTimeHeader"];

        var rowIndex = 2;
        foreach (var factory in factories)
        {
            worksheet.Cell(rowIndex, 1).Value = factory.FactoryCode;
            worksheet.Cell(rowIndex, 2).Value = factory.FactoryName;
            worksheet.Cell(rowIndex, 3).Value = factory.FactoryDesc;
            worksheet.Cell(rowIndex, 4).Value = FromFlag(factory.Enabled) ? "Y" : "N";
            worksheet.Cell(rowIndex, 5).Value = factory.UpdateTime;
            worksheet.Cell(rowIndex, 5).Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var exportStream = new MemoryStream();
        workbook.SaveAs(exportStream);
        exportStream.Position = 0;

        var fileName = string.Format(_localizer["ExportFileName"], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        return File(exportStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    private static string ToFlag(bool value) => value ? "Y" : "N";

    private static bool FromFlag(string? value) => string.Equals(value, "Y", StringComparison.OrdinalIgnoreCase);

    private static SysHtFactory CreateHistory(SysFactory entity) => new()
    {
        FactoryId = entity.FactoryId,
        FactoryCode = entity.FactoryCode,
        FactoryName = entity.FactoryName,
        FactoryDesc = entity.FactoryDesc,
        Enabled = entity.Enabled,
        UpdateTime = entity.UpdateTime ?? DateTime.UtcNow,
        UpdateUserId = entity.UpdateUserId
    };

    private async Task<decimal> GenerateFactoryIdAsync(CancellationToken cancellationToken)
    {
        var currentMax = await _db.Factories.MaxAsync(f => (decimal?)f.FactoryId, cancellationToken) ?? 0m;
        var trackedMax = _db.ChangeTracker.Entries<SysFactory>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity.FactoryId)
            .DefaultIfEmpty(0m)
            .Max();

        return Math.Max(currentMax, trackedMax) + 1m;
    }
}

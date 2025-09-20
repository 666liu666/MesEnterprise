
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MesEnterprise.Application.Common.Interfaces;

namespace MesEnterprise.Infrastructure.Persistence;

public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly MesDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public EfRepository(MesDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<(IReadOnlyCollection<TEntity> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsNoTracking();
        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

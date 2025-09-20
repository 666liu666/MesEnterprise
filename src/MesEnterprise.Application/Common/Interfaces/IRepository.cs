namespace MesEnterprise.Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<(IReadOnlyCollection<TEntity> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
}

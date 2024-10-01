namespace CrawlerStorage.Data.Common.Interfaces;

using System.Linq.Expressions;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task AddAsync(TEntity entity);

    IQueryable<TEntity> All();

    IQueryable<TEntity> AllAsNoTracking();

    Task<TEntity> FindAsync(object id);

    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression);

    Task<int> SaveChangesAsync();

    void Update(TEntity entity);

    void Delete(TEntity entity);
}
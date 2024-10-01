namespace CrawlerStorage.Data.Repositories;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using CrawlerStorage.Data.Common.Interfaces;

using Microsoft.EntityFrameworkCore;

public class CrawlerStorageRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    public CrawlerStorageRepository(CrawlerStorageDbContext context)
    {
        this.Context = context;
        this.DbSet = this.Context.Set<TEntity>();
    }

    protected CrawlerStorageDbContext Context { get; }

    protected DbSet<TEntity> DbSet { get; }

    public async Task AddAsync(TEntity entity)
    {
        await this.DbSet.AddAsync(entity).AsTask();
    }

    public IQueryable<TEntity> All()
    {
        return this.DbSet;
    }

    public IQueryable<TEntity> AllAsNoTracking()
    {
        return this.DbSet.AsNoTracking();
    }

    public void Delete(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> FindAsync(object id)
    {
        return await this.DbSet.FindAsync(id);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await this.DbSet.Where(expression).FirstOrDefaultAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await this.Context.SaveChangesAsync();
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }
}
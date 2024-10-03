namespace CrawlerStorage.Data;

using CrawlerStorage.Data.Common.Interfaces;
using CrawlerStorage.Data.Models.DbEntities;

using Microsoft.EntityFrameworkCore;

public class CrawlerStorageDbContext : DbContext
{
    public CrawlerStorageDbContext(DbContextOptions<CrawlerStorageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Crawler> Crawlers { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Operation> Operations { get; set; }

    public override int SaveChanges()
    {
        return this.SaveChanges(true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ApplyCheckRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return this.SaveChangesAsync(true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.ApplyCheckRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyCheckRules()
    {
        var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e => e.Entity is ICheckableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in changedEntries)
        {
            var entity = (ICheckableEntity)entry.Entity;
            if (entry.State == EntityState.Added && entity.CreatedOn == default)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
            else
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
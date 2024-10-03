namespace CrawlerStorage.Data;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Enumerations;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class PrepareDatabase
{
    public static void Population(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        SeedData(serviceScope.ServiceProvider.GetService<CrawlerStorageDbContext>());
    }

    private static void SeedData(CrawlerStorageDbContext context)
    {
        context.Database.Migrate();

        if (!context.Operations.Any())
        {
            context.AddRange(
               new Operation { Name = nameof(OperationType.None) },
               new Operation { Name = nameof(OperationType.Add) },
               new Operation { Name = nameof(OperationType.Update) },
               new Operation { Name = nameof(OperationType.Delete) },
               new Operation { Name = nameof(OperationType.Error) }
            );

            context.SaveChanges();
        }
    }
}
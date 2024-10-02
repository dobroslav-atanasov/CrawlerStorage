namespace CrawlerStorage.Data;

using System;

using Microsoft.EntityFrameworkCore;

public static class PrepareDatabase
{
    public static void Migrate(CrawlerStorageDbContext context)
    {
        context.Database.Migrate();
    }

    private static void SeedData(CrawlerStorageDbContext context)
    {
        throw new NotImplementedException();
    }
}
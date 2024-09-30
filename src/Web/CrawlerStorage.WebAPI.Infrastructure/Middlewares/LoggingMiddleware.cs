namespace CrawlerStorage.WebAPI.Infrastructure.Middlewares;

using Microsoft.AspNetCore.Http;

public class LoggingMiddleware
{
    private readonly RequestDelegate next;

    public LoggingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            await this.LogError(ex);
            throw;
        }
    }

    private async Task LogError(Exception ex)
    {
        throw new NotImplementedException();
    }
}
using System.Reflection;

using Asp.Versioning;

using CrawlerStorage.Common.Constants;
using CrawlerStorage.Data;
using CrawlerStorage.Data.Repositories;
using CrawlerStorage.Services;
using CrawlerStorage.Services.Intrefaces;
using CrawlerStorage.WebAPI.Infrastructure.Middlewares;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new HeaderApiVersionReader(GlobalConstants.API_VERSION_HEADER));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = GlobalConstants.API_VERSION_NAME;
    options.SubstituteApiVersionInUrl = true;
});

// Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(GlobalConstants.LOG_FILE_PATH, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// Automapper
builder.Services.AddAutoMapper(Assembly.Load(GlobalConstants.ASSEMBLY_AUTOMAPPER));

// Database
builder.Services.AddDbContext<CrawlerStorageDbContext>(options =>
{
    options.UseLazyLoadingProxies();

    if (builder.Environment.IsDevelopment())
    {
        //options.UseSqlServer(builder.Configuration.GetConnectionString(GlobalConstants.CRAWLER_STORAGE_CONNECTION_STRING));
        options.UseInMemoryDatabase("InMemory");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString(GlobalConstants.CRAWLER_STORAGE_CONNECTION_STRING));
        //options.UseNpgsql(builder.Configuration.GetConnectionString(GlobalConstants.CRAWLER_STORAGE_CONNECTION_STRING));
    }
});

// Repositories
builder.Services.AddScoped(typeof(CrawlerStorageRepository<>));

// Services
builder.Services.AddHttpClient<IGroupsService, GroupsService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc(GlobalConstants.API_VERSION_1,
        new OpenApiInfo { Title = $"{GlobalConstants.API_VERSION_TITLE} {GlobalConstants.API_VERSION_1}", Version = $"{GlobalConstants.API_VERSION_1}.0" });
});

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (builder.Environment.IsProduction())
{
    PrepareDatabase.Population(app);
}

app.Run();
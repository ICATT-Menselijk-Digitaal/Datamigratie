// use `dotnet run generate` to generate the json schema. we do this in a github action

using Datamigratie.Data;
using Datamigratie.MigrationService;
using Datamigratie.MigrationService.Features.DatabaseInitialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddDatamigratieDbContext();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();


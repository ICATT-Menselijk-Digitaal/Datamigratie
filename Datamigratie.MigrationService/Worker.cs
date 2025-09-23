using System.Diagnostics;
using Datamigratie.MigrationService.Features.DatabaseInitialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Datamigratie.MigrationService;

/// <summary>
/// A background worker that handles EF Core migrations and loads initial data from a JSON dataset.
/// If migrations fail, the process exits with a non-zero code to prevent application startup.
/// See https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations
/// </summary>
public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    // Used for distributed tracing and diagnostics
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    /// <summary>
    /// Entry point of the background worker.
    /// Runs the database migration and loads the dataset within a transaction.
    /// If any error occurs, logs the exception and halts the application startup.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            await initializer.Initialize(cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            logger.LogCritical(ex, "Migrations failed");
            // Exit process with error code to block PABC.Server startup
            Environment.ExitCode = 1;
        }
        finally
        {
            // Gracefully shut down the host application once work is complete
            hostApplicationLifetime.StopApplication();
        }
    }
}

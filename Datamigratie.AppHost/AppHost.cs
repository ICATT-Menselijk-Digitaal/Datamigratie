using System.Net.Http.Json;
using Datamigratie.AppHost;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var detApiKey = builder.AddParameter("DetApiKey", "super-secret", true);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithHostPort(63214)
    .WithPgAdmin(x => x.WithHostPort(63215).WithLifetime(ContainerLifetime.Persistent))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("postgis/postgis");

var redis = builder.AddRedis("redis");

var dmtDb = postgres.AddDatabase("Datamigratie");

var ozaakdb = postgres.AddDatabase("openzaakdb", "open_zaak");


var migrations = builder
    .AddProject<Projects.Datamigratie_MigrationService>("migrations")
    .WithReference(dmtDb)
    .WaitFor(dmtDb);

var det = builder.AddProject<Projects.Datamigratie_FakeDet>("datamigratie-fakedet")
    .WithEnvironment("ApiKey", detApiKey)
    .WithHttpCommand("zaken", "clear zaken", commandOptions: new() { Method = HttpMethod.Delete })
    .WithHttpCommand("zaken", "generate zaken", commandOptions: new()
    {
        Method = HttpMethod.Post,
        PrepareRequest = async (ctx) =>
        {
#pragma warning disable ASPIREINTERACTION001
            var interactionService = ctx.ServiceProvider.GetRequiredService<IInteractionService>();
            var input = await interactionService.PromptInputAsync(title: "Number of zaken to generate", message: null, input: new()
            {
                Name = "zaken",
                InputType = InputType.Number,
                Label = "Zaken",
                Placeholder = "100"
            }, cancellationToken: ctx.CancellationToken);
            if (int.TryParse(input.Data?.Value, out var count))
            {
                ctx.Request.Content = JsonContent.Create(count);
            }
        }
    });
#pragma warning restore ASPIREINTERACTION001

var openzaak = builder.AddOpenZaak("openzaak")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithReference(ozaakdb)
    .WithReference(redis)
    .WithInitialSettings("ConfigInladen", Path.Combine("openzaak", "config.yaml"))
    .WaitFor(ozaakdb)
    .WaitFor(redis);

openzaak.AddInitScript(ozaakdb, "CatalogiInladen", Path.Combine("openzaak", "data__dump.sql"));

var proxy = openzaak.AddNginxProxy("OpenZaakProxy");

builder.AddProject<Projects.Datamigratie_Server>("datamigratie-server")
    .WithEnvironment("OpenZaakApi__BaseUrl", $"{proxy.GetEndpoint("http")}/")
    .WithEnvironment("OpenZaakApi__ApiKey", "super-secret-with-a-lot-of-characters")
    .WithEnvironment("OpenZaakApi__ApiUser", "user-id")
    .WithEnvironment("DetApi__BaseUrl", det.GetEndpoint("http"))
    .WithEnvironment("DetApi__ApiKey", detApiKey)
    .WithReference(dmtDb)
    .WaitFor(dmtDb)
    .WaitFor(openzaak)
    .WaitFor(det)
    .WaitForCompletion(migrations);

builder.Build().Run();

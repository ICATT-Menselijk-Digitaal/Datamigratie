
var builder = DistributedApplication.CreateBuilder(args);

var detApiKey = builder.AddParameter("DetApiKey", "super-secret", true);

var postgres = builder.AddPostgres("postgis")
    .WithDataVolume()
    .WithHostPort(63214)
    .WithPgAdmin(x => x.WithHostPort(63215).WithLifetime(ContainerLifetime.Persistent))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("postgis/postgis")
    .WithImageTag("17-3.6-alpine");

var redis = builder.AddRedis("redis").WithLifetime(ContainerLifetime.Persistent);

var dmtDb = postgres.AddDatabase("Datamigratie");

var ozaakdb = postgres.AddDatabase("openzaakdb", "open_zaak");


var migrations = builder
    .AddProject<Projects.Datamigratie_MigrationService>("migrations")
    .WithReference(dmtDb)
    .WaitFor(dmtDb);

var det = builder.AddProject<Projects.Datamigratie_FakeDet>("datamigratie-fakedet")
    .WithEnvironment("ApiKey", detApiKey);

var openzaak = builder.AddOpenZaak("openzaak", port: 54322)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithReference(ozaakdb)
    .WithReference(redis)
    .WithInitialSettings("ConfigInladen", Path.Combine("openzaak", "config.yaml"))
    .WaitFor(ozaakdb)
    .WaitFor(redis);

openzaak.AddInitScript(ozaakdb, "CatalogiInladen", Path.Combine("openzaak", "data__dump.sql"));

var proxy = openzaak.AddNginxProxy("OpenZaakProxy", 54321)
    .WithLifetime(ContainerLifetime.Persistent);

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

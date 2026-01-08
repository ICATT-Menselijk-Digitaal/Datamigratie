using Datamigratie.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

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

builder.AddProject<Projects.Datamigratie_FakeDet>("datamigratie-fakedet");

var openzaak = builder.AddOpenZaak("openzaak")
    .WithReference(ozaakdb)
    .WithReference(redis)
    .WithInitialSettings("ConfigInladen", Path.Combine("openzaak", "config.yaml"))
    .WaitFor(ozaakdb)
    .WaitFor(redis);

openzaak.AddInitScript(ozaakdb, "CatalogiInladen", Path.Combine("openzaak", "data__dump.sql"));

var proxy = openzaak.AddNginxProxy("OpenZaakProxy");

builder.AddProject<Projects.Datamigratie_Server>("datamigratie-server")
    .WithReference(dmtDb)
    .WaitFor(dmtDb)
    .WaitForCompletion(migrations);

builder.Build().Run();

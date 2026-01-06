using Datamigratie.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithHostPort(63214)
    .WithPgAdmin(x => x.WithHostPort(63215).WithLifetime(ContainerLifetime.Persistent))
    .WithLifetime(ContainerLifetime.Persistent)
    .WithImage("postgis/postgis");

var redis = builder.AddRedis("redis");

var postgresdb = postgres.AddDatabase("Datamigratie");
var ozaakdb = postgres.AddDatabase("OpenZaakDb", "open_zaak");

var migrations = builder
    .AddProject<Projects.Datamigratie_MigrationService>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Datamigratie_FakeDet>("datamigratie-fakedet");

var openzaak = builder.AddOpenZaak("openzaak")
    .WithReference(ozaakdb)
    .WithReference(redis)
    .WaitFor(ozaakdb)
    .WaitFor(redis);

var proxy = openzaak.AddNginxProxy("OpenZaakProxy");

builder.AddProject<Projects.Datamigratie_Server>("datamigratie-server")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);

builder.Build().Run();

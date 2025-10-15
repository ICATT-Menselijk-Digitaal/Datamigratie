var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithHostPort(63214)
    .WithPgAdmin(x => x.WithHostPort(63215));

var postgresdb = postgres.AddDatabase("Datamigratie");

var migrations = builder
    .AddProject<Projects.Datamigratie_MigrationService>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddProject<Projects.Datamigratie_Server>("datamigratie-server")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WaitForCompletion(migrations);


builder.Build().Run();

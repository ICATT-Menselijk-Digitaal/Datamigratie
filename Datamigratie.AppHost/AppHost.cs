var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Datamigratie_Server>("datamigratie-server");

builder.Build().Run();

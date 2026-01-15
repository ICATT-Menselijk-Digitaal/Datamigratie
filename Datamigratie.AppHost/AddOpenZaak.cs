namespace Datamigratie.AppHost;

public static class AddOpenZaakExtensions
{
    public static IResourceBuilder<OpenZaakResource> AddOpenZaak(this IDistributedApplicationBuilder builder, string name, string tag = "latest")
    {
        var resource = new OpenZaakResource(name);

        var resourceBuilder = builder
            .AddResource(resource)
            .WithHttpEndpoint(targetPort: 8000);
        var endpoint = resourceBuilder.GetEndpoint("http");

        return resourceBuilder
            .WithImage("openzaak/open-zaak", tag)
            .WithImageRegistry("docker.io")
            .WithHttpHealthCheck("/admin")
            .WithOtlpExporter()
            .WithCertificateTrustConfiguration(ctx =>
            {
                ctx.EnvironmentVariables["EXTRA_VERIFY_CERTS"] = ctx.CertificateBundlePath;
                ctx.EnvironmentVariables["OTEL_EXPORTER_OTLP_CERTIFICATE"] = ctx.CertificateBundlePath;
                return Task.CompletedTask;
            })
            .WithEnvironment("DJANGO_SETTINGS_MODULE", "openzaak.conf.docker")
            .WithEnvironment("SECRET_KEY", "7(h1r2hk)8z9+05edulo_3qzymwbo&c24=)qz7+_@3&2sp=u%i")
            .WithEnvironment("IS_HTTPS", "no")
            .WithEnvironment("SITE_DOMAIN", $"localhost:{endpoint.Property(EndpointProperty.Port)}")
            .WithEnvironment("ALLOWED_HOSTS", $"localhost,127.0.0.1,{endpoint.Property(EndpointProperty.Host)}")
            .WithEnvironment("CORS_ALLOW_ALL_ORIGINS", "True")
            .WithEnvironment("SUBPATH", "/")
            .WithEnvironment("IMPORT_DOCUMENTEN_BASE_DIR", "/app/import-data")
            .WithEnvironment("IMPORT_DOCUMENTEN_BATCH_SIZE", "500")
            .WithEnvironment("OPENZAAK_SUPERUSER_USERNAME", "admin")
            .WithEnvironment("DJANGO_SUPERUSER_PASSWORD", "admin")
            .WithEnvironment("OPENZAAK_SUPERUSER_EMAIL", "admin@localhost")
            .WithEnvironment("DISABLE_2FA", "true")
            .WithEnvironment("ENVIRONMENT", "dev")
            .WithEnvironment("NOTIFICATIONS_DISABLED", "True");
    }

    public static IResourceBuilder<OpenZaakResource> WithReference(this IResourceBuilder<OpenZaakResource> builder, IResourceBuilder<PostgresDatabaseResource> database)
    {
        var postgres = database.Resource.Parent;

        if (postgres.UserNameParameter is null)
        {
            var parameter = builder.ApplicationBuilder.AddParameter("OzaakDbUser", "postgres");
            postgres.UserNameParameter = parameter.Resource;
        }

        return builder
            .WithEnvironment("DB_USER", postgres.UserNameParameter)
            .WithEnvironment("DB_HOST", postgres.Host)
            .WithEnvironment("DB_NAME", database.Resource.GetConnectionProperty("DatabaseName"))
            .WithEnvironment("DB_PASSWORD", database.Resource.Parent.PasswordParameter)
            .WithEnvironment("DB_CONN_MAX_AGE", "0")
            .WithEnvironment("DB_POOL_ENABLED", "True");
    }

    public static IResourceBuilder<OpenZaakResource> WithReference(this IResourceBuilder<OpenZaakResource> builder, IResourceBuilder<RedisResource> redis)
    {
        var endpoint = redis.Resource.GetEndpoint("secondary");
        var hostAndPort = endpoint.Property(EndpointProperty.HostAndPort);
        var password = redis.Resource.PasswordParameter;

        var cacheUrl = password is null
            ? ReferenceExpression.Create($"{hostAndPort}/0")
            : ReferenceExpression.Create($":{password}@{hostAndPort}/0");

        return builder
            .WithEnvironment("CACHE_DEFAULT", cacheUrl)
            .WithEnvironment("CACHE_AXES", cacheUrl);
    }

    public static IResourceBuilder<ContainerResource> AddNginxProxy(this IResourceBuilder<OpenZaakResource> openzaak, string name)
    {
        var nginx = openzaak.ApplicationBuilder
            .AddContainer(name, "nginx")
            .WithHttpEndpoint(targetPort: 80)
            .WithEnvironment("OPEN_ZAAK_REFERENCE", openzaak.GetEndpoint("http"))
            .WithBindMount("nginx/templates/default.conf.template", "/etc/nginx/templates/default.conf.template")
            .WaitFor(openzaak)
            .WithParentRelationship(openzaak);

        var endpoint = nginx.GetEndpoint("http");

        openzaak
            .WithEnvironment("CSRF_TRUSTED_ORIGINS", () => $"http://localhost:{endpoint.Port}")
            .WithEnvironment("OPENZAAK_DOMAIN", () => $"localhost:{endpoint.Port}")
            .WithEnvironment("OPENZAAK_REWRITE_HOST", "True");

        return nginx;
    }

    public static IResourceBuilder<PostgresServerResource> AddInitScript(this IResourceBuilder<OpenZaakResource> openzaak, IResourceBuilder<PostgresDatabaseResource> builder, string name, string path)
    {
        const string TargetScript = "/init-script/init.sql";

        var runner = builder.ApplicationBuilder.AddPostgres(name)
            .WithBindMount(path, TargetScript)
            .WithEnvironment("PGPASSWORD", builder.Resource.Parent.PasswordParameter)
            .WithEnvironment("PGDATABASE", builder.Resource.DatabaseName)
            .WithEnvironment("PGHOST", builder.Resource.Parent.Host)
            .WithArgs("psql", "-f", TargetScript)
            .WaitFor(builder)
            .WaitFor(openzaak)
            .WithParentRelationship(openzaak);

        if(builder.Resource.Parent.UserNameParameter is {} user)
        {
            runner.WithEnvironment("PGUSER", user);
        }
        else
        {
            runner.WithEnvironment("PGUSER", "postgres");
        }

        return runner;
    }

    public static IResourceBuilder<OpenZaakResource> WithInitialSettings(this IResourceBuilder<OpenZaakResource> openzaak, string name, string path)
    {
        const string TargetFile = "/app/setup_configuration/data.yaml";

        var runner = openzaak
            .ApplicationBuilder
            .AddOpenZaak(name)
            .WithBindMount(path, TargetFile)
            .WithEnvironment(async ctx =>
            {
                if(openzaak.Resource.TryGetEnvironmentVariables(out var variables))
                {
                    foreach (var item in variables)
                    {
                        await item.Callback(ctx);
                    }
                }
            })
            .WithEnvironment("RUN_SETUP_CONFIG", "True")
            .WithArgs("/setup_configuration.sh")
            .WithParentRelationship(openzaak)
            .WaitFor(openzaak);
        
        return openzaak;
    }
}

public class OpenZaakResource : ContainerResource
{
    internal OpenZaakResource(string name, string? entrypoint = null) : base(name, entrypoint)
    {
    }
}

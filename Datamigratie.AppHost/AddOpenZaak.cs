using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Datamigratie.AppHost
{
    internal static class AddOpenZaakExtensions
    {
        public static IResourceBuilder<OpenZaakResource> AddOpenZaak(this IDistributedApplicationBuilder builder, string name, string tag = "latest")
        {
            var resource = new OpenZaakResource(name);

            builder.Services.AddHttpClient(name).ConfigurePrimaryHttpMessageHandler((a, b) =>
            {
                if (a is HttpClientHandler handler)
                {
                    handler.AllowAutoRedirect = false;
                }
            }).ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(new Uri(resource.GetEndpoint("http").Url), "admin");
            });

            return builder
                .AddResource(resource)
                .WithImage("openzaak/open-zaak", tag)
                .WithImageRegistry("docker.io")
                .WithHttpEndpoint(targetPort: 8000)
                .WithHttpHealthCheck("/admin")
                .WithOtlpExporter()
                .WithEnvironment("DJANGO_SETTINGS_MODULE", "openzaak.conf.docker")
                .WithEnvironment("SECRET_KEY", "7(h1r2hk)8z9+05edulo_3qzymwbo&c24=)qz7+_@3&2sp=u%i")
                .WithEnvironment("IS_HTTPS", "no")
                .WithEnvironment((context) =>
                {
                    //var dashboard = builder.Resources.OfType<ExecutableResource>().First(x => x.Name == "aspire-dashboard");
                    //var otlp = dashboard.GetEndpoint("otlp-grpc");
                    //context.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = otlp;
                    var e = resource.GetEndpoints().First();
                    context.EnvironmentVariables["SITE_DOMAIN"] = $"localhost:{e.Port}";
                })
                .WithEnvironment("ALLOWED_HOSTS", $"localhost,127.0.0.1,{resource.GetEndpoint("http").Property(EndpointProperty.Host)}")
                .WithEnvironment("CORS_ALLOW_ALL_ORIGINS", "True")

                //.WithEnvironment("CACHE_DEFAULT", "redis:6379/0")
                //.WithEnvironment("CACHE_AXES", "redis:6379/0")
                .WithEnvironment("SUBPATH", "/")
                .WithEnvironment("IMPORT_DOCUMENTEN_BASE_DIR", "/app/import-data")
                .WithEnvironment("IMPORT_DOCUMENTEN_BATCH_SIZE", "500")
                .WithEnvironment("OPENZAAK_SUPERUSER_USERNAME", "admin")
                .WithEnvironment("DJANGO_SUPERUSER_PASSWORD", "admin")
                .WithEnvironment("OPENZAAK_SUPERUSER_EMAIL", "admin@localhost")
                .WithEnvironment("DISABLE_2FA", "true")
                //.WithEnvironment("CELERY_BROKER_URL", "redis://redis:6379/1")
                //.WithEnvironment("CELERY_RESULT_BACKEND", "redis://redis:6379/1")
                //.WithEnvironment("CELERY_RESULT_EXPIRES", "3600")
                //.WithEnvironment("CELERY_LOGLEVEL", "DEBUG")
                //.WithEnvironment("CELERY_WORKER_CONCURRENCY", "4")
                .WithEnvironment("ENVIRONMENT", "dev")
                //.WithEnvironment("OTEL_SDK_DISABLED", "true")
                //.WithEnvironment("LOG_NOTIFICATIONS_IN_DB", "yes")
                ;
        }

        public static IResourceBuilder<OpenZaakResource> WithDatabase(this IResourceBuilder<OpenZaakResource> builder, IResourceBuilder<PostgresDatabaseResource> database)
        {
            if (database.Resource.Parent.UserNameParameter is { })
            {
                builder.WithEnvironment("DB_USER", database.Resource.Parent.UserNameParameter);
            }
            else
            {
                builder.WithEnvironment("DB_USER", "postgres");
            }

            //DB_HOST
            return builder
                .WithEnvironment("DB_HOST", database.Resource.Parent.Host)
                .WithEnvironment("DB_NAME", database.Resource.DatabaseName)
                .WithEnvironment("DB_PASSWORD", database.Resource.Parent.PasswordParameter)
                .WithEnvironment("DB_CONN_MAX_AGE", "0")
                .WithEnvironment("DB_POOL_ENABLED", "True");
        }

        public static IResourceBuilder<ContainerResource> AddProxy(this IResourceBuilder<OpenZaakResource> openzaak, string name)
        {
            return openzaak.ApplicationBuilder
                .AddContainer(name, "nginx")
                .WithHttpEndpoint(targetPort: 80)
                .WithEnvironment("OPEN_ZAAK_REFERENCE", openzaak.GetEndpoint("http"))
                .WithBindMount("nginx/templates/default.conf.template", "/etc/nginx/templates/default.conf.template")
                .WaitFor(openzaak)
                .WithParentRelationship(openzaak);
        }

        public static IResourceBuilder<OpenZaakResource> WithRedis(this IResourceBuilder<OpenZaakResource> builder, IResourceBuilder<RedisResource> redis)
        {
            return builder.WithEnvironment(context =>
            {
                var endpoint = redis.Resource.GetEndpoint("secondary");
                var cacheUrl = ReferenceExpression.Create($":{redis.Resource.PasswordParameter}@{endpoint.Property(EndpointProperty.HostAndPort)}/0");
                context.EnvironmentVariables["CACHE_DEFAULT"] = cacheUrl;
                context.EnvironmentVariables["CACHE_AXES"] = cacheUrl;
            });
        }
    }

    public class OpenZaakResource : ContainerResource, IResourceWithServiceDiscovery
    {
        internal OpenZaakResource(string name, string? entrypoint = null) : base(name, entrypoint)
        {
        }
    }
}

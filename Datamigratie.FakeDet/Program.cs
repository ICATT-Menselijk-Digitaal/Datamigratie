using System.Text.Json.Serialization;
using Datamigratie.FakeDet;
using Datamigratie.FakeDet.DataGeneration.Zaken;
using Microsoft.AspNetCore.Mvc;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<ZakenGenerator>();
builder.Services.AddSingleton<ZaakDatabase>();

var app = builder.Build();

app.MapDefaultEndpoints();

var group = app.MapGroup("/").AddEndpointFilter(async (context, next) =>
{
    if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
    {
        return Results.Unauthorized();
    }
    if (!extractedApiKey.Equals(app.Configuration["ApiKey"]))
    {
        return Results.Forbid();
    }
    return await next(context);
});

group.MapGet("zaaktypen", FakeDetEndpoints.GetAllZaaktypen);
group.MapGet("zaaktypen/{zaaktypeName}", FakeDetEndpoints.GetZaaktype);
group.MapGet("zaken", FakeDetEndpoints.GetZakenByZaaktype);
group.MapGet("zaken/{zaaknummer}", FakeDetEndpoints.GetZaak);
group.MapGet("documenten/inhoud/{id}", FakeDetEndpoints.DownloadBestand);
group.MapGet("documentstatussen", FakeDetEndpoints.GetAllDocumentStatussen);
group.MapDelete("zaken", (ZakenGenerator generator) => generator.Delete());
group.MapPost("zaken", async (ZakenGenerator generator, [FromBody] int? count) => await generator.Generate(count));

await app.Services.GetRequiredService<ZakenGenerator>().Generate();

app.Run();


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

app.MapGet("zaaktypen", FakeDetEndpoints.GetAllZaaktypen);
app.MapGet("zaaktypen/{zaaktypeName}", FakeDetEndpoints.GetZaaktype);
app.MapGet("zaken", FakeDetEndpoints.GetZakenByZaaktype);
app.MapGet("zaken/{zaaknummer}", FakeDetEndpoints.GetZaak);
app.MapGet("documenten/inhoud/{id}", FakeDetEndpoints.DownloadBestand);
app.MapGet("documentstatussen", FakeDetEndpoints.GetAllDocumentStatussen);
app.MapDelete("zaken", (ZakenGenerator generator) => generator.Delete());
app.MapPost("zaken", async (ZakenGenerator generator, [FromBody] int? count) => await generator.Generate(count));

await app.Services.GetRequiredService<ZakenGenerator>().Generate();

app.Run();


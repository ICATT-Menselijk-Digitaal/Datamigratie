using System.Text.Json.Serialization;
using Datamigratie.FakeDet;
using Datamigratie.FakeDet.Catalogi;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<JsonOptions>(opts =>
{
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("zaaktypen", FakeDetEndpoints.GetAllZaaktypen);
app.MapGet("zaaktypen/{zaaktypeName}", FakeDetEndpoints.GetZaaktype);
app.MapGet("zaken", FakeDetEndpoints.GetZakenByZaaktype);
app.MapGet("zaken/{zaaknummer}", FakeDetEndpoints.GetZaak);
app.MapGet("documenten/inhoud/{id}", FakeDetEndpoints.DownloadBestand);
app.MapGet("test", GetCatalogusData.GetCatalogi);


app.Run();


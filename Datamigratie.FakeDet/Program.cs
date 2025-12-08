using Datamigratie.FakeDet;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

app.Run();


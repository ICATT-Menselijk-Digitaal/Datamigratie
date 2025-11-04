using Datamigratie.Data;
using Datamigratie.Server.Auth;
using Datamigratie.Server.Config;
using Datamigratie.Server.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatamigratieDbContext();

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddAuth(options =>
 {
        options.Authority = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:Authority");
        options.ClientId = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientId");
        options.ClientSecret = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientSecret");
        options.FunctioneelBeheerderRole = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:FunctioneelBeheerderRole");
        options.NameClaimType = builder.Configuration["Oidc:NameClaimType"];
        options.RoleClaimType = builder.Configuration["Oidc:RoleClaimType"];
        options.EmailClaimType = builder.Configuration["Oidc:EmailClaimType"];
 });

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapDmtAuthEndpoints();

app.MapControllers();

app.MapFallbackToFile("/index.html").AllowAnonymous();

app.Run();

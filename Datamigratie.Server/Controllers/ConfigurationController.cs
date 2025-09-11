using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Datamigratie.Server.Controllers
{
    public record AppVersion(string? Version, string? Revision, string? ReleaseVariable, string? SecretVariable);

    [ApiController]
    public class AppVersionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AppVersionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("api/app-version", Name = "GetAppVersion")]
        public ActionResult<AppVersion> Get()
        {
            var parts = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion?.Split('+') ?? [];
            
            var appVersion = new AppVersion(
                parts.ElementAtOrDefault(0) ?? "0.0.0",
                parts.ElementAtOrDefault(1) ?? "dev",
                _configuration["RELEASE_VARIABLE"] ?? "Not configured",
                _configuration["SECRET_VARIABLE"] ?? "Not configured"
            );
                
            return new OkObjectResult(appVersion);
        }
    }
}
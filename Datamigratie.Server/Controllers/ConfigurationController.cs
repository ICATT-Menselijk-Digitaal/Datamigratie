using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Datamigratie.Server.Controllers
{
    public record AppVersion(string? Version, string? Revision, bool EnableTestHelpers);

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
            
            var enableTestHelpers = _configuration.GetValue<bool>("FeatureFlags:EnableTestHelpers", false);
            
            var appVersion = new AppVersion(
                parts.ElementAtOrDefault(0) ?? "0.0.0",
                parts.ElementAtOrDefault(1) ?? "dev",
                enableTestHelpers
            );
                
            return new OkObjectResult(appVersion);
        }
    }
}
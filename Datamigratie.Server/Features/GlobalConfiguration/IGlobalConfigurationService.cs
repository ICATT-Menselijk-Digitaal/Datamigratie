using Datamigratie.Server.Features.GlobalConfiguration.Models;

namespace Datamigratie.Server.Features.GlobalConfiguration;

public interface IGlobalConfigurationService
{
    Task<GlobalConfigurationResponse> GetConfigurationAsync();
    Task<GlobalConfigurationResponse> UpdateConfigurationAsync(UpdateGlobalConfigurationRequest request);
    Task<string?> GetRsinAsync();
}

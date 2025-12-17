using Datamigratie.Data;
using Datamigratie.Server.Features.GlobalConfiguration.Models;
using Datamigratie.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.GlobalConfiguration;

public class GlobalConfigurationService(
    DatamigratieDbContext dbContext,
    ILogger<GlobalConfigurationService> logger) : IGlobalConfigurationService
{
    public async Task<GlobalConfigurationResponse> GetConfigurationAsync()
    {
        var config = await dbContext.GlobalConfigurations.FirstOrDefaultAsync();

        if (config == null)
        {
            return new GlobalConfigurationResponse();
        }

        return new GlobalConfigurationResponse
        {
            Rsin = config.Rsin,
            UpdatedAt = config.UpdatedAt
        };
    }

    public async Task<GlobalConfigurationResponse> UpdateConfigurationAsync(UpdateGlobalConfigurationRequest request)
    {
        // Validate RSIN if provided
        if (!string.IsNullOrWhiteSpace(request.Rsin))
        {
            if (!RsinValidator.IsValid(request.Rsin))
            {
                var error = RsinValidator.GetValidationError(request.Rsin);
                logger.LogWarning("Invalid RSIN provided: {Rsin}. Error: {Error}", request.Rsin, error);
                throw new InvalidOperationException(error);
            }
        }

        var config = await dbContext.GlobalConfigurations.FirstOrDefaultAsync();

        if (config == null)
        {
            // Create new configuration
            config = new Data.Entities.GlobalConfiguration
            {
                Rsin = request.Rsin,
                UpdatedAt = DateTime.UtcNow
            };
            dbContext.GlobalConfigurations.Add(config);
            logger.LogInformation("Creating new global configuration with RSIN: {Rsin}", request.Rsin);
        }
        else
        {
            // Update existing configuration
            config.Rsin = request.Rsin;
            config.UpdatedAt = DateTime.UtcNow;
            logger.LogInformation("Updating global configuration with RSIN: {Rsin}", request.Rsin);
        }

        await dbContext.SaveChangesAsync();

        return new GlobalConfigurationResponse
        {
            Rsin = config.Rsin,
            UpdatedAt = config.UpdatedAt
        };
    }

    public async Task<string?> GetRsinAsync()
    {
        var config = await dbContext.GlobalConfigurations.FirstOrDefaultAsync();
        return config?.Rsin;
    }
}

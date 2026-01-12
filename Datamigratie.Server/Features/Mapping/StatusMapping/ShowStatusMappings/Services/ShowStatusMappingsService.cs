using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Services;

public interface IShowStatusMappingsService
{
    Task<StatusMappingsResponse> GetStatusMappings(string detZaaktypeId);
}

public class ShowStatusMappingsService(DatamigratieDbContext context) : IShowStatusMappingsService
{
    public async Task<StatusMappingsResponse> GetStatusMappings(string detZaaktypeId)
    {
        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

        if (zaaktypenMapping == null)
        {
            return new StatusMappingsResponse { Mappings = [] };
        }

        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        return new StatusMappingsResponse
        {
            Mappings = existingMappings
                .Select(em => new StatusMappingDto
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })
                .ToList()
        };
    }
}

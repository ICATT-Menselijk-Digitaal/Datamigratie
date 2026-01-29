using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Services;

public interface IShowStatusMappingsService
{
    Task<List<StatusMappingsResponse>> GetStatusMappings(Guid zaaktypenMappingId);
}

public class ShowStatusMappingsService(DatamigratieDbContext context) : IShowStatusMappingsService
{
    public async Task<List<StatusMappingsResponse>> GetStatusMappings(Guid zaaktypenMappingId)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        return [.. existingMappings
                .Select(em => new StatusMappingsResponse
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })];
    }
}

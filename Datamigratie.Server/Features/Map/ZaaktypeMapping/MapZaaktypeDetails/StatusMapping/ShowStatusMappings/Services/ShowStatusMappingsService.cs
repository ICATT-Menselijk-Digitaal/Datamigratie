using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.StatusMapping.ShowStatusMappings.Services;

public interface IShowStatusMappingsService
{
    Task<List<StatusMappingsResponse>> GetStatusMappings(Guid zaaktypeVersieMappingId);
}

public class ShowStatusMappingsService(DatamigratieDbContext context) : IShowStatusMappingsService
{
    public async Task<List<StatusMappingsResponse>> GetStatusMappings(Guid zaaktypeVersieMappingId)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .ToListAsync();

        return [.. existingMappings
                .Select(em => new StatusMappingsResponse
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })];
    }
}

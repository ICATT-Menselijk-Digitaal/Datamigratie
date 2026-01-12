using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.ShowStatusMappings.Services;

public interface IShowStatusMappingsService
{
    Task<StatusMappingsResponse> GetStatusMappings(Guid zaaktypenMappingId);
}

public class ShowStatusMappingsService(DatamigratieDbContext context) : IShowStatusMappingsService
{
    public async Task<StatusMappingsResponse> GetStatusMappings(Guid zaaktypenMappingId)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        return new StatusMappingsResponse
        {
            Mappings = [.. existingMappings
                .Select(em => new StatusMappingDto
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })]
        };
    }
}

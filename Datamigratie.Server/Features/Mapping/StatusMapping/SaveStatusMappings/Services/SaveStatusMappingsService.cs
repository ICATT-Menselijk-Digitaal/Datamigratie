using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.SaveStatusMappings.Services;

public interface ISaveStatusMappingsService
{
    Task SaveStatusMappings(SaveStatusMappingsRequest request);
}

public class SaveStatusMappingsService(DatamigratieDbContext context) : ISaveStatusMappingsService
{
    public async Task SaveStatusMappings(SaveStatusMappingsRequest request)
    {
        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == request.DetZaaktypeId);

        if (zaaktypenMapping == null)
        {
            throw new InvalidOperationException($"No zaaktypen mapping found for DetZaaktypeId: {request.DetZaaktypeId}");
        }

        var existingMappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();
        
        context.StatusMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.StatusMapping
        {
            ZaaktypenMappingId = zaaktypenMapping.Id,
            DetStatusNaam = m.DetStatusNaam,
            OzStatustypeId = m.OzStatustypeId
        });
        
        await context.StatusMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

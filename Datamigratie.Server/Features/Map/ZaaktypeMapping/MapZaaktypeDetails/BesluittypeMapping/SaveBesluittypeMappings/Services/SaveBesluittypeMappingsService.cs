using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.SaveBesluittypeMappings.Services;

public interface ISaveBesluittypeMappingsService
{
    Task SaveBesluittypeMappings(Guid zaaktypenMappingId, SaveBesluittypeMappingsRequest request);
}

public class SaveBesluittypeMappingsService(DatamigratieDbContext context) : ISaveBesluittypeMappingsService
{
    public async Task SaveBesluittypeMappings(Guid zaaktypenMappingId, SaveBesluittypeMappingsRequest request)
    {
        var existingMappings = await context.BesluittypeMappings
            .Where(bm => bm.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();
        
        context.BesluittypeMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

        var newMappings = request.Mappings.Select(m => new Data.Entities.BesluittypeMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetBesluittypeNaam = m.DetBesluittypeNaam,
            OzBesluittypeId = m.OzBesluittypeId
        });
        
        await context.BesluittypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

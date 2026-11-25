using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings;

public interface ISaveDocumenttypeMappingsService
{
    Task SaveDocumenttypeMappings(Guid zaaktypenMappingId, SaveDocumenttypeMappingsRequest request);
}

public class SaveDocumenttypeMappingsService(DatamigratieDbContext context) : ISaveDocumenttypeMappingsService
{
    public async Task SaveDocumenttypeMappings(Guid zaaktypenMappingId, SaveDocumenttypeMappingsRequest request)
    {
        var existingMappings = await context.DocumenttypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.DocumenttypeMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

        var newMappings = request.Mappings.Select(m => new Data.Entities.DocumenttypeMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetDocumenttypeNaam = m.DetDocumenttypeNaam,
            OzInformatieobjecttypeUrl = m.OzInformatieobjecttypeUrl
        });

        await context.DocumenttypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

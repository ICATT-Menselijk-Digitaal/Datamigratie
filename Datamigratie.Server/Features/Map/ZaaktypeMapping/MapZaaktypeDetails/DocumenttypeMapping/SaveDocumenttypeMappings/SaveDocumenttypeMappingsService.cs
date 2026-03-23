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
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.DocumenttypeMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.DocumenttypeMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetDocumenttypeNaam = m.DetDocumenttypeNaam,
            OzInformatieobjecttypeUrl = m.OzInformatieobjecttypeUrl
        });

        await context.DocumenttypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

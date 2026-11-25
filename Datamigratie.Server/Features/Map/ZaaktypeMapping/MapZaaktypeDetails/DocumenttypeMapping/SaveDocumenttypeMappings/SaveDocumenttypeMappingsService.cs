using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.SaveDocumenttypeMappings;

public interface ISaveDocumenttypeMappingsService
{
    Task SaveDocumenttypeMappings(Guid zaaktypeVersieMappingId, SaveDocumenttypeMappingsRequest request);
}

public class SaveDocumenttypeMappingsService(DatamigratieDbContext context) : ISaveDocumenttypeMappingsService
{
    public async Task SaveDocumenttypeMappings(Guid zaaktypeVersieMappingId, SaveDocumenttypeMappingsRequest request)
    {
        var existingMappings = await context.DocumenttypeMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .ToListAsync();

        context.DocumenttypeMappings.RemoveRange(existingMappings);

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

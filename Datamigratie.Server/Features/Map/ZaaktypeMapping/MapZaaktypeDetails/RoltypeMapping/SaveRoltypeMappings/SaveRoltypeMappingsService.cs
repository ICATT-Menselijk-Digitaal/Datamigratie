using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;

public interface ISaveRoltypeMappingsService
{
    Task SaveRoltypeMappings(Guid zaaktypenMappingId, SaveRoltypeMappingsRequest request);
}

public class SaveRoltypeMappingsService(DatamigratieDbContext context) : ISaveRoltypeMappingsService
{
    public async Task SaveRoltypeMappings(Guid zaaktypenMappingId, SaveRoltypeMappingsRequest request)
    {
        var existingMappings = await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.RoltypeMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.RoltypeMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetRol = m.DetRol,
            OzRoltypeUrl = m.OzRoltypeUrl
        });

        await context.RoltypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

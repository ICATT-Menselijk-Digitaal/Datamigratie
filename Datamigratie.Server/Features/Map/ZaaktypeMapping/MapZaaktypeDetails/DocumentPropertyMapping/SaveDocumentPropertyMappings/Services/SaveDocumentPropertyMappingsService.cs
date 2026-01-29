using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.SaveDocumentPropertyMappings.Services;

public interface ISaveDocumentPropertyMappingsService
{
    Task SaveDocumentPropertyMappings(Guid zaaktypenMappingId, SaveDocumentPropertyMappingsRequest request);
}

public class SaveDocumentPropertyMappingsService(DatamigratieDbContext context) : ISaveDocumentPropertyMappingsService
{
    public async Task SaveDocumentPropertyMappings(Guid zaaktypenMappingId, SaveDocumentPropertyMappingsRequest request)
    {
        var existingMappings = await context.DocumentPropertyMappings
            .Where(dam => dam.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();
        
        context.DocumentPropertyMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.DocumentPropertyMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetPropertyName = m.DetPropertyName,
            DetValue = m.DetValue,
            OzValue = m.OzValue
        });
        
        await context.DocumentPropertyMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

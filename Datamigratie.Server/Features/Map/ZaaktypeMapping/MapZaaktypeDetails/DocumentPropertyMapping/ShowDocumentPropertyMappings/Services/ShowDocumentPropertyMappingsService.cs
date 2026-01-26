using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.ShowDocumentPropertyMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.ShowDocumentPropertyMappings.Services;

public interface IShowDocumentPropertyMappingsService
{
    Task<List<DocumentPropertyMappingResponse>> GetDocumentPropertyMappings(Guid zaaktypenMappingId);
}

public class ShowDocumentPropertyMappingsService(DatamigratieDbContext context) : IShowDocumentPropertyMappingsService
{
    public async Task<List<DocumentPropertyMappingResponse>> GetDocumentPropertyMappings(Guid zaaktypenMappingId)
    {
        var mappings = await context.DocumentPropertyMappings
            .Where(dam => dam.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(dam => new DocumentPropertyMappingResponse
            {
                DetPropertyName = dam.DetPropertyName,
                DetValue = dam.DetValue,
                OzValue = dam.OzValue
            })
            .ToListAsync();

        return mappings;
    }
}

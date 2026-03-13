using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings;

public interface IShowDocumenttypeMappingsService
{
    Task<List<DocumenttypeMappingResponse>> GetDocumenttypeMappings(Guid zaaktypenMappingId);
}

public class ShowDocumenttypeMappingsService(DatamigratieDbContext context) : IShowDocumenttypeMappingsService
{
    public async Task<List<DocumenttypeMappingResponse>> GetDocumenttypeMappings(Guid zaaktypenMappingId)
    {
        return [.. await context.DocumenttypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(m => new DocumenttypeMappingResponse
            {
                DetDocumenttypeNaam = m.DetDocumenttypeNaam,
                OzInformatieobjecttypeUrl = m.OzInformatieobjecttypeUrl
            })
            .ToListAsync()];
    }
}

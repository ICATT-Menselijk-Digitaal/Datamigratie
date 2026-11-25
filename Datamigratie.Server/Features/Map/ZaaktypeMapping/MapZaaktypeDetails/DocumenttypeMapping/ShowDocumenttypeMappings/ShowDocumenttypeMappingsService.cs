using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumenttypeMapping.ShowDocumenttypeMappings;

public interface IShowDocumenttypeMappingsService
{
    Task<List<DocumenttypeMappingResponse>> GetDocumenttypeMappings(Guid zaaktypeVersieMappingId);
}

public class ShowDocumenttypeMappingsService(DatamigratieDbContext context) : IShowDocumenttypeMappingsService
{
    public async Task<List<DocumenttypeMappingResponse>> GetDocumenttypeMappings(Guid zaaktypeVersieMappingId)
    {
        return [.. await context.DocumenttypeMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .Select(m => new DocumenttypeMappingResponse
            {
                DetDocumenttypeNaam = m.DetDocumenttypeNaam,
                OzInformatieobjecttypeUrl = m.OzInformatieobjecttypeUrl
            })
            .ToListAsync()];
    }
}

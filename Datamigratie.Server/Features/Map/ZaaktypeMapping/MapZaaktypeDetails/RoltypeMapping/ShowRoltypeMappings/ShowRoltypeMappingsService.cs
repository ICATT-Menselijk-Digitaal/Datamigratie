using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;

public interface IShowRoltypeMappingsService
{
    Task<List<RoltypeMappingResponse>> GetRoltypeMappings(Guid zaaktypenMappingId);
}

public class ShowRoltypeMappingsService(DatamigratieDbContext context) : IShowRoltypeMappingsService
{
    public async Task<List<RoltypeMappingResponse>> GetRoltypeMappings(Guid zaaktypenMappingId)
    {
        return await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(m => new RoltypeMappingResponse
            {
                DetRol = m.DetRol,
                OzRoltypeUrl = m.OzRoltypeUrl
            })
            .ToListAsync();
    }
}

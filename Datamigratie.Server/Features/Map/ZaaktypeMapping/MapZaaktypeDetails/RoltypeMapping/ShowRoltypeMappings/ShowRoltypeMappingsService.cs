using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;

public interface IShowRoltypeMappingsService
{
    Task<List<RoltypeMappingResponse>> GetRoltypeMappings(Guid zaaktypeVersieMappingId);
}

public class ShowRoltypeMappingsService(DatamigratieDbContext context) : IShowRoltypeMappingsService
{
    public async Task<List<RoltypeMappingResponse>> GetRoltypeMappings(Guid zaaktypeVersieMappingId)
    {
        return await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .Select(m => new RoltypeMappingResponse
            {
                DetRol = m.DetRol,
                AlleenPdf = m.AlleenPdf,
                OzRoltypeUrl = m.OzRoltypeUrl
            })
            .ToListAsync();
    }
}

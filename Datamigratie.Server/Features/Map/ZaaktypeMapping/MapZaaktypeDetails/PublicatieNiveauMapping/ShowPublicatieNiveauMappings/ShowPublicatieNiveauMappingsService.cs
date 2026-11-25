using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.ShowPublicatieNiveauMappings;

public interface IShowPublicatieNiveauMappingsService
{
    Task<List<PublicatieNiveauMappingResponse>> GetPublicatieNiveauMappings(Guid zaaktypeVersieMappingId);
}

public class ShowPublicatieNiveauMappingsService(DatamigratieDbContext context) : IShowPublicatieNiveauMappingsService
{
    public async Task<List<PublicatieNiveauMappingResponse>> GetPublicatieNiveauMappings(Guid zaaktypeVersieMappingId)
    {
        return [.. await context.PublicatieNiveauMappings
            .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .Select(m => new PublicatieNiveauMappingResponse
            {
                DetPublicatieNiveau = m.DetPublicatieNiveau,
                OzVertrouwelijkheidaanduiding = m.OzVertrouwelijkheidaanduiding
            })
            .ToListAsync()];
    }
}

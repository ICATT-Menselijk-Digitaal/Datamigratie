using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PublicatieNiveauMapping.SavePublicatieNiveauMappings;

public interface ISavePublicatieNiveauMappingsService
{
    Task SavePublicatieNiveauMappings(Guid zaaktypenMappingId, SavePublicatieNiveauMappingsRequest request);
}

public class SavePublicatieNiveauMappingsService(DatamigratieDbContext context) : ISavePublicatieNiveauMappingsService
{
    public async Task SavePublicatieNiveauMappings(Guid zaaktypenMappingId, SavePublicatieNiveauMappingsRequest request)
    {
        var existingMappings = await context.PublicatieNiveauMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.PublicatieNiveauMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.PublicatieNiveauMapping
        {
            ZaaktypenMappingId = zaaktypenMappingId,
            DetPublicatieNiveau = m.DetPublicatieNiveau,
            OzVertrouwelijkheidaanduiding = m.OzVertrouwelijkheidaanduiding
        });

        await context.PublicatieNiveauMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

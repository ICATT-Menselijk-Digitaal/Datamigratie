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
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.PublicatieNiveauMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

        var newMappings = request.Mappings.Select(m => new Data.Entities.PublicatieNiveauMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetPublicatieNiveau = m.DetPublicatieNiveau,
            OzVertrouwelijkheidaanduiding = m.OzVertrouwelijkheidaanduiding
        });

        await context.PublicatieNiveauMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

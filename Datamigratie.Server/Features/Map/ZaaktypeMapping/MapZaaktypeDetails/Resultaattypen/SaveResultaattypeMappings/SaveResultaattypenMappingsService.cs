using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings
{
    public interface ISaveResultaattypenMappingsService
    {
        Task SaveResultaattypeMappings(Guid zaaktypenMappingId, SaveResultaattypeMappingRequest request);
    }

    public class SaveResultaattypenMappingsService(DatamigratieDbContext context) : ISaveResultaattypenMappingsService
    {
        public async Task SaveResultaattypeMappings(Guid zaaktypenMappingId, SaveResultaattypeMappingRequest request)
        {
            var existingMappings = await context.ResultaattypeMappings
                .Where(sm => sm.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
                .ToListAsync();
            
            context.ResultaattypeMappings.RemoveRange(existingMappings);

            // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
            var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
                .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
                .Select(v => v.Id)
                .SingleAsync();

            var newMappings = request.Mappings.Select(m => new Data.Entities.ResultaattypeMapping
            {
                ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
                DetResultaattypeNaam = m.DetResultaattypeNaam,
                OzResultaattypeId = m.OzResultaattypeId
            });
            
            await context.ResultaattypeMappings.AddRangeAsync(newMappings);
            await context.SaveChangesAsync();
        }
    }
}

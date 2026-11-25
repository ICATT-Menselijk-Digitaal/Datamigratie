using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.SaveResultaattypeMappings
{
    public interface ISaveResultaattypenMappingsService
    {
        Task SaveResultaattypeMappings(Guid zaaktypeVersieMappingId, SaveResultaattypeMappingRequest request);
    }

    public class SaveResultaattypenMappingsService(DatamigratieDbContext context) : ISaveResultaattypenMappingsService
    {
        public async Task SaveResultaattypeMappings(Guid zaaktypeVersieMappingId, SaveResultaattypeMappingRequest request)
        {
            var existingMappings = await context.ResultaattypeMappings
                .Where(sm => sm.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
                .ToListAsync();
            
            context.ResultaattypeMappings.RemoveRange(existingMappings);

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

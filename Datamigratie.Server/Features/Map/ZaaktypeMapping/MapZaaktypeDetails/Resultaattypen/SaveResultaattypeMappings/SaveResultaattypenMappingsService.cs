using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.SaveResultaattypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.Resultaattypen.SaveResultaattypeMappings
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
                .Where(sm => sm.ZaaktypenMappingId == zaaktypenMappingId)
                .ToListAsync();
            
            context.ResultaattypeMappings.RemoveRange(existingMappings);

            var newMappings = request.Mappings.Select(m => new Data.Entities.ResultaattypeMapping
            {
                ZaaktypenMappingId = zaaktypenMappingId,
                DetResultaattypeNaam = m.DetResultaattypeNaam,
                OzResultaattypeId = m.OzResultaattypeId
            });
            
            await context.ResultaattypeMappings.AddRangeAsync(newMappings);
            await context.SaveChangesAsync();
        }
    }
}

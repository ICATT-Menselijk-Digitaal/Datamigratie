using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    public interface IMapResultaattypenService
    {
        Task CreateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozResultaattypeId);

        Task UpdateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid updatedOzResultaattypeId);
    }

    public class MapResultaattypenService(DatamigratieDbContext context) : IMapResultaattypenService
    {
        public async Task CreateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozResultaattypeId)
        {
            // Look up the ZaaktypenMapping based on DetZaaktypeId
            var zaaktypenMapping = await context.Mappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .FirstOrDefaultAsync();

            if (zaaktypenMapping == null)
            {
                throw new InvalidOperationException($"No ZaaktypenMapping found for DET Zaaktype '{detZaaktypeId}'. Please create a zaaktype mapping first.");
            }

            var mapping = new ResultaattypeMapping
            {
                ZaaktypenMappingId = zaaktypenMapping.Id,
                DetResultaattypeId = detResultaattypeId,
                OzResultaattypeId = ozResultaattypeId
            };

            context.ResultaattypeMappings.Add(mapping);
            await context.SaveChangesAsync();
        }

        public async Task UpdateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid updatedOzResultaattypeId)
        {
            // Look up the ZaaktypenMapping based on DetZaaktypeId
            var zaaktypenMapping = await context.Mappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .FirstOrDefaultAsync();

            if (zaaktypenMapping == null)
            {
                throw new InvalidOperationException($"No ZaaktypenMapping found for DET Zaaktype '{detZaaktypeId}'.");
            }

            var rowsAffected = await context.ResultaattypeMappings
                .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id && m.DetResultaattypeId == detResultaattypeId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(x => x.OzResultaattypeId, updatedOzResultaattypeId));

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Mapping for DET Resultaattype '{detResultaattypeId}' in zaaktype '{detZaaktypeId}' does not exist.");
            }
        }
    }
}

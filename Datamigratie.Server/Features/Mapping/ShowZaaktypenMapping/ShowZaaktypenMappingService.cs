
using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.MapZaaktypen.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ShowZaaktypenMapping
{
    public interface IShowZaaktypenMappingService
    {
        Task<ZaaktypenMapping> GetZaaktypenMapping(string detZaaktypeId);
    }

    public class ShowZaaktypenMappingService(DatamigratieDbContext context) : IShowZaaktypenMappingService
    {
        public async Task<ZaaktypenMapping> GetZaaktypenMapping(string detZaaktypeId)
        {
            var zaaktypenMappingEntity = await context.Mappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

            if (zaaktypenMappingEntity == null)
            {
                throw new Exception($"Mapping for Det zaaktype ID '{detZaaktypeId}' does not exist.");
            }

            var zaaktypenMapping = new ZaaktypenMapping
            {
                DetZaaktypeId = zaaktypenMappingEntity.DetZaaktypeId,
                OzZaaktypeId = zaaktypenMappingEntity.OzZaaktypeId
            };

            return zaaktypenMapping;
        }
    }
}

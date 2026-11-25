using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping
{
    public interface IShowZaaktypenMappingService
    {
        Task<ZaaktypenMapping?> GetZaaktypenMapping(string detZaaktypeId);
    }

    public class ShowZaaktypenMappingService(DatamigratieDbContext context) : IShowZaaktypenMappingService
    {
        public async Task<ZaaktypenMapping?> GetZaaktypenMapping(string detZaaktypeId)
        {
            var zaaktypenMappingEntity = await context.Mappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

            if (zaaktypenMappingEntity == null)
            {
                return null;
            }

            var zaaktypenMapping = new ZaaktypenMapping
            {
                Id = zaaktypenMappingEntity.Id,
                DetZaaktypeId = zaaktypenMappingEntity.DetZaaktypeId,
                OzZaaktypeId = zaaktypenMappingEntity.OzZaaktypeId,
                Versies = await context.ZaaktypeVersieMappings
                    .Where(v => v.ZaaktypenMappingId == zaaktypenMappingEntity.Id)
                    .OrderBy(v => v.SelectielijstJaar)
                    .Select(v => new ZaaktypeVersie
                    {
                        Id = v.Id,
                        SelectielijstJaar = v.SelectielijstJaar,
                        OzZaaktypeVersieId = v.OzZaaktypeVersieId
                    })
                    .ToListAsync()
            };

            return zaaktypenMapping;
        }
    }
}

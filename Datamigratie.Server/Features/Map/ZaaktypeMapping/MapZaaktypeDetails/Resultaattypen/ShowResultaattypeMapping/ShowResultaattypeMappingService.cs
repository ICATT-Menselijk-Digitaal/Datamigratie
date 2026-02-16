using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping
{
    public interface IShowResultaattypeMappingService
    {
        Task<List<ResultaattypeMappingResponse>> GetResultaattypenMappings(Guid zaaktypenMappingId);
    }

    public class ShowResultaattypeMappingService(DatamigratieDbContext context) : IShowResultaattypeMappingService
    {
        public async Task<List<ResultaattypeMappingResponse>> GetResultaattypenMappings(Guid zaaktypenMappingId)
        {
            var resultaattypeMappingEntities = await context.ResultaattypeMappings
                .Include(m => m.ZaaktypenMapping)
                .Where(m => m.ZaaktypenMapping.Id == zaaktypenMappingId)
                .ToListAsync();

            return [.. resultaattypeMappingEntities.Select(entity => new ResultaattypeMappingResponse
            {
                DetResultaattypeNaam = entity.DetResultaattypeNaam,
                OzResultaattypeId = entity.OzResultaattypeId
            })];
        }
    }
}

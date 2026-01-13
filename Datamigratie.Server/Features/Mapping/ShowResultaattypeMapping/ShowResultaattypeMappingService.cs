using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
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

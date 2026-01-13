using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    public interface IShowResultaattypeMappingService
    {
        Task<List<ResultaattypeMappingResponse>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId);
    }

    public class ShowResultaattypeMappingService(DatamigratieDbContext context) : IShowResultaattypeMappingService
    {
        public async Task<List<ResultaattypeMappingResponse>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId)
        {
            var resultaattypeMappingEntities = await context.ResultaattypeMappings
                .Include(m => m.ZaaktypenMapping)
                .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktypeId)
                .ToListAsync();

            return [.. resultaattypeMappingEntities.Select(entity => new ResultaattypeMappingResponse
            {
                DetResultaattypeId = entity.DetResultaattypeId,
                OzResultaattypeId = entity.OzResultaattypeId
            })];
        }
    }
}

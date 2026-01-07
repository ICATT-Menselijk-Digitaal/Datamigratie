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
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .ToListAsync();

            return [.. resultaattypeMappingEntities.Select(entity => new ResultaattypeMappingResponse
            {
                DetZaaktypeId = entity.DetZaaktypeId,
                DetResultaattypeId = entity.DetResultaattypeId,
                OzZaaktypeId = entity.OzZaaktypeId,
                OzResultaattypeId = entity.OzResultaattypeId
            })];
        }
    }
}

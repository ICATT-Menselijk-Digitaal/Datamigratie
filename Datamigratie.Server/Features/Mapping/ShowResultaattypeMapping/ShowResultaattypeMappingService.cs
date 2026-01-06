using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    public interface IShowResultaattypeMappingService
    {
        Task<ResultaattypeMappingResponse?> GetResultaattypeMapping(string detZaaktypeId);
        Task<List<ResultaattypeMappingResponse>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId);
    }

    public class ShowResultaattypeMappingService(DatamigratieDbContext context) : IShowResultaattypeMappingService
    {
        public async Task<ResultaattypeMappingResponse?> GetResultaattypeMapping(string detZaaktypeId)
        {
            var resultaattypeMappingEntity = await context.ResultaattypeMappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId);

            if (resultaattypeMappingEntity == null)
            {
                return null;
            }

            var resultaattypeMapping = new ResultaattypeMappingResponse
            {
                DetZaaktypeId = resultaattypeMappingEntity.DetZaaktypeId,
                DetResultaattypeId = resultaattypeMappingEntity.DetResultaattypeId,
                OzZaaktypeId = resultaattypeMappingEntity.OzZaaktypeId,
                OzResultaattypeId = resultaattypeMappingEntity.OzResultaattypeId
            };

            return resultaattypeMapping;
        }

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

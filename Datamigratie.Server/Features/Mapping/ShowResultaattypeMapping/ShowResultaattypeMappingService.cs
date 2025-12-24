using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ShowResultaattypeMapping
{
    public interface IShowResultaattypeMappingService
    {
        Task<ResultaattypeMapping?> GetResultaattypeMapping(string detZaaktypeId, string detResultaattypeId);
        Task<List<ResultaattypeMapping>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId);
    }

    public class ShowResultaattypeMappingService(DatamigratieDbContext context) : IShowResultaattypeMappingService
    {
        public async Task<ResultaattypeMapping?> GetResultaattypeMapping(string detZaaktypeId, string detResultaattypeId)
        {
            var resultaattypeMappingEntity = await context.ResultaattypeMappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId && m.DetResultaattypeId == detResultaattypeId);

            if (resultaattypeMappingEntity == null)
            {
                return null;
            }

            var resultaattypeMapping = new ResultaattypeMapping
            {
                DetZaaktypeId = resultaattypeMappingEntity.DetZaaktypeId,
                DetResultaattypeId = resultaattypeMappingEntity.DetResultaattypeId,
                OzZaaktypeId = resultaattypeMappingEntity.OzZaaktypeId,
                OzResultaattypeId = resultaattypeMappingEntity.OzResultaattypeId
            };

            return resultaattypeMapping;
        }

        public async Task<List<ResultaattypeMapping>> GetAllResultaattypeMappingsForZaaktype(string detZaaktypeId)
        {
            var resultaattypeMappingEntities = await context.ResultaattypeMappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .ToListAsync();

            return resultaattypeMappingEntities.Select(entity => new ResultaattypeMapping
            {
                DetZaaktypeId = entity.DetZaaktypeId,
                DetResultaattypeId = entity.DetResultaattypeId,
                OzZaaktypeId = entity.OzZaaktypeId,
                OzResultaattypeId = entity.OzResultaattypeId
            }).ToList();
        }
    }
}

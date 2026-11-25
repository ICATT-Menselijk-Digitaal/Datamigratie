using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.Resultaattypen.ShowResultaattypeMapping
{
    public interface IShowResultaattypeMappingService
    {
        Task<List<ResultaattypeMappingResponse>> GetResultaattypenMappings(Guid zaaktypeVersieMappingId);
    }

    public class ShowResultaattypeMappingService(DatamigratieDbContext context) : IShowResultaattypeMappingService
    {
        public async Task<List<ResultaattypeMappingResponse>> GetResultaattypenMappings(Guid zaaktypeVersieMappingId)
        {
            var resultaattypeMappingEntities = await context.ResultaattypeMappings
                .Where(m => m.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
                .ToListAsync();

            return [.. resultaattypeMappingEntities.Select(entity => new ResultaattypeMappingResponse
            {
                DetResultaattypeNaam = entity.DetResultaattypeNaam,
                OzResultaattypeId = entity.OzResultaattypeId
            })];
        }
    }
}

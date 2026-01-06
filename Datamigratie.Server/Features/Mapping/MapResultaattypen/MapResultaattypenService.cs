using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    public interface IMapResultaattypenService
    {
        Task CreateResultaattypeMapping(string detZaaktypeId, Guid ozZaaktypeId, Guid ozResultaattypeId);

        Task UpdateResultaattypeMapping(string detZaaktypeId, Guid ozZaaktypeId, Guid updatedOzResultaattypeId);
    }

    public class MapResultaattypenService(DatamigratieDbContext context) : IMapResultaattypenService
    {
        public async Task CreateResultaattypeMapping(string detZaaktypeId, Guid ozZaaktypeId, Guid ozResultaattypeId)
        {
            var mapping = new ResultaattypeMapping
            {
                DetZaaktypeId = detZaaktypeId,
                OzZaaktypeId = ozZaaktypeId,
                OzResultaattypeId = ozResultaattypeId
            };

            context.ResultaattypeMappings.Add(mapping);
            await context.SaveChangesAsync();
        }

        public async Task UpdateResultaattypeMapping(string detZaaktypeId, Guid ozZaaktypeId, Guid updatedOzResultaattypeId)
        {
            var rowsAffected = await context.ResultaattypeMappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(x => x.OzZaaktypeId, ozZaaktypeId)
                    .SetProperty(x => x.OzResultaattypeId, updatedOzResultaattypeId));

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Mapping for DET zaaktype '{detZaaktypeId}' does not exist.");
            }
        }
    }
}

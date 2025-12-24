using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.MapResultaattypen
{
    public interface IMapResultaattypenService
    {
        Task CreateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozZaaktypeId, Guid ozResultaattypeId);

        Task UpdateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozZaaktypeId, Guid updatedOzResultaattypeId);
    }

    public class MapResultaattypenService(DatamigratieDbContext context) : IMapResultaattypenService
    {
        public async Task CreateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozZaaktypeId, Guid ozResultaattypeId)
        {
            // TODO: Add validation for DET and OZ Resultaattypen when API methods are available
            // await ValidateDetResultaattypeExistsAsync(detZaaktypeId, detResultaattypeId);
            // await ValidateOzResultaattypeExistsAsync(ozZaaktypeId, ozResultaattypeId);

            var existingMapping = await context.ResultaattypeMappings
                .AnyAsync(m => m.DetZaaktypeId == detZaaktypeId && m.DetResultaattypeId == detResultaattypeId);

            if (existingMapping)
            {
                throw new InvalidOperationException($"Mapping for DET Resultaattype '{detResultaattypeId}' in zaaktype '{detZaaktypeId}' already exists, update instead");
            }

            var mapping = new ResultaattypeMapping
            {
                DetZaaktypeId = detZaaktypeId,
                DetResultaattypeId = detResultaattypeId,
                OzZaaktypeId = ozZaaktypeId,
                OzResultaattypeId = ozResultaattypeId
            };

            context.ResultaattypeMappings.Add(mapping);
            await context.SaveChangesAsync();
        }

        public async Task UpdateResultaattypeMapping(string detZaaktypeId, string detResultaattypeId, Guid ozZaaktypeId, Guid updatedOzResultaattypeId)
        {
            // TODO: Add validation for OZ Resultaattype when API method is available
            // await ValidateOzResultaattypeExistsAsync(ozZaaktypeId, updatedOzResultaattypeId);

            var rowsAffected = await context.ResultaattypeMappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId && m.DetResultaattypeId == detResultaattypeId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(x => x.OzZaaktypeId, ozZaaktypeId)
                    .SetProperty(x => x.OzResultaattypeId, updatedOzResultaattypeId));

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Mapping for DET Resultaattype '{detResultaattypeId}' in zaaktype '{detZaaktypeId}' does not exist.");
            }
        }

        // TODO: Implement validation methods when API clients support Resultaattypen
        // private async Task ValidateOzResultaattypeExistsAsync(Guid ozZaaktypeId, Guid ozResultaattypeId)
        // {
        //     var resultaattype = await openZaakApiClient.GetResultaattypeById(ozResultaattypeId);
        //     if (resultaattype == null)
        //     {
        //         throw new InvalidOperationException($"OpenZaak Resultaattype with ID '{ozResultaattypeId}' was not found");
        //     }
        // }

        // private async Task ValidateDetResultaattypeExistsAsync(string detZaaktypeId, string detResultaattypeId)
        // {
        //     var resultaattype = await detApiClient.GetResultaattype(detZaaktypeId, detResultaattypeId);
        //     if (resultaattype == null)
        //     {
        //         throw new InvalidOperationException($"DET Resultaattype with ID '{detResultaattypeId}' was not found");
        //     }
        // }
    }
}

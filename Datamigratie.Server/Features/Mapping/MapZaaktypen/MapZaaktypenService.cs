using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.MapZaaktypen
{
    public interface IMapZaaktypenService
    {
        Task CreateZaaktypenMapping(string detZaaktypeId, Guid ozZaaktypeId);

        Task UpdateZaaktypenMapping(string detZaaktypeId, Guid newOzZaaktypeId);
    }

    public class MapZaaktypenService(IOpenZaakApiClient openZaakApiClient, IDetApiClient detApiClient, DatamigratieDbContext context) : IMapZaaktypenService
    {
        public async Task CreateZaaktypenMapping(string detZaaktypeId, Guid ozZaaktypeId)
        {
            await ValidateDetZaaktypeExistsAsync(detZaaktypeId);
            await ValidateOzZaaktypeExistsAsync(ozZaaktypeId);

            var existingMapping = await context.Mappings
                .AnyAsync(m => m.DetZaaktypeId == detZaaktypeId);

            if (existingMapping)
            {
                throw new InvalidOperationException($"Mapping for Det zaaktype ID '{detZaaktypeId}' already exists, update instead");
            }

            var mapping = new ZaaktypeMapping
            {
                DetZaaktypeId = detZaaktypeId,
                OzZaaktypeId = ozZaaktypeId
            };

            context.Mappings.Add(mapping);
            await context.SaveChangesAsync();
        }

        public async Task UpdateZaaktypenMapping(string detZaaktypeId, Guid newOzZaaktypeId)
        {
            await ValidateOzZaaktypeExistsAsync(newOzZaaktypeId);

            var rowsAffected = await context.Mappings
                .Where(m => m.DetZaaktypeId == detZaaktypeId)
                .ExecuteUpdateAsync(m => m.SetProperty(x => x.OzZaaktypeId, newOzZaaktypeId));

            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Mapping for Det zaaktype ID '{detZaaktypeId}' does not exist.");
            }
        }

        private async Task ValidateOzZaaktypeExistsAsync(Guid ozZaaktypeId)
        {
            try
            {
                await openZaakApiClient.GetZaaktype(ozZaaktypeId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"OpenZaak zaaktype with ID '{ozZaaktypeId}' was not found. Exception: {ex.Message}");
            }
        }

        private async Task ValidateDetZaaktypeExistsAsync(string detZaaktypeId)
        {
            try
            {
                await detApiClient.GetZaaktype(detZaaktypeId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Det zaaktype with ID '{detZaaktypeId}' was not found. Exception: {ex.Message}");
            }
        }
    }
}

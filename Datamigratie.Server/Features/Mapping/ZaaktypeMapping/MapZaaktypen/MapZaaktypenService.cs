using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.ZaaktypeMapping.MapZaaktypen
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

            var mapping = new ZaaktypenMapping
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

            var currentMapping = await context.Mappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId) ?? throw new InvalidOperationException($"Mapping for Det zaaktype ID '{detZaaktypeId}' does not exist.");

            // delete status mappings if the OZ zaaktype is being changed
            if (currentMapping.OzZaaktypeId != newOzZaaktypeId)
            {
                var statusMappings = await context.StatusMappings
                    .Where(sm => sm.ZaaktypenMappingId == currentMapping.Id)
                    .ToListAsync();

                if (statusMappings.Count != 0)
                {
                    context.StatusMappings.RemoveRange(statusMappings);
                    await context.SaveChangesAsync();
                }

                var resultaattypeMappings = await context.ResultaattypeMappings
                   .Where(sm => sm.ZaaktypenMappingId == currentMapping.Id)
                   .ToListAsync();

                if (resultaattypeMappings.Count != 0)
                {
                    context.ResultaattypeMappings.RemoveRange(resultaattypeMappings);
                    await context.SaveChangesAsync();
                }
            }

            currentMapping.OzZaaktypeId = newOzZaaktypeId;
            await context.SaveChangesAsync();
        }

        private async Task ValidateOzZaaktypeExistsAsync(Guid ozZaaktypeId)
        {
            var zaaktype = await openZaakApiClient.GetZaaktype(ozZaaktypeId);

            if (zaaktype == null)
            {
                throw new InvalidOperationException($"OpenZaak zaaktype with ID '{ozZaaktypeId}' was not found");
            }
        }

        private async Task ValidateDetZaaktypeExistsAsync(string detZaaktypeId)
        {
            var zaaktype = await detApiClient.GetZaaktype(detZaaktypeId);

            if (zaaktype == null)
            {
                throw new InvalidOperationException($"OpenZaak zaaktype with ID '{detZaaktypeId}' was not found");
            }
        }
    }
}

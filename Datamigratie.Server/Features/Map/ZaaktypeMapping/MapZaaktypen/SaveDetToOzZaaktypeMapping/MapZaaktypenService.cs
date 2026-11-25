using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Selectielijst;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping
{
    public interface IMapZaaktypenService
    {
        Task CreateZaaktypenMapping(string detZaaktypeId, Guid ozZaaktypeId);

        Task UpdateZaaktypenMapping(string detZaaktypeId, Guid newOzZaaktypeId);
    }

    public class MapZaaktypenService(
        IOpenZaakApiClient openZaakApiClient,
        IDetApiClient detApiClient,
        ISelectielijstApiClient selectielijstApiClient,
        DatamigratieDbContext context) : IMapZaaktypenService
    {
        public async Task CreateZaaktypenMapping(string detZaaktypeId, Guid ozZaaktypeId)
        {
            await ValidateDetZaaktypeExistsAsync(detZaaktypeId);
            var ozZaaktype = await GetOzZaaktypeAsync(ozZaaktypeId);

            var existingMapping = await context.Mappings
                .AnyAsync(m => m.DetZaaktypeId == detZaaktypeId);

            if (existingMapping)
            {
                throw new InvalidOperationException($"Mapping for Det zaaktype ID '{detZaaktypeId}' already exists, update instead");
            }

            var mappingId = Guid.NewGuid();
            var versies = await GetNewestVersiePerSelectielijstJaarAsync(ozZaaktype, mappingId);

            context.Mappings.Add(new ZaaktypenMapping
            {
                Id = mappingId,
                DetZaaktypeId = detZaaktypeId,
                OzZaaktypeId = ozZaaktypeId,
                OzZaaktypeIdentificatie = ozZaaktype.Identificatie,
                OzCatalogusId = OzUrlToGuidConverter.ExtractUuidFromUrl(ozZaaktype.Catalogus),
                Versies = versies
            });

            await context.SaveChangesAsync();
        }

        public async Task UpdateZaaktypenMapping(string detZaaktypeId, Guid newOzZaaktypeId)
        {
            var newOzZaaktype = await GetOzZaaktypeAsync(newOzZaaktypeId);

            var currentMapping = await context.Mappings
                .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId) ?? throw new InvalidOperationException($"Mapping for Det zaaktype ID '{detZaaktypeId}' does not exist.");

            var newCatalogusId = OzUrlToGuidConverter.ExtractUuidFromUrl(newOzZaaktype.Catalogus);

            // Another version of the same OZ zaaktype yields the same versions per selectielijst year, so the mappings stay
            var isSameOzZaaktype = currentMapping.OzZaaktypeIdentificatie == newOzZaaktype.Identificatie
                && currentMapping.OzCatalogusId == newCatalogusId;

            if (!isSameOzZaaktype)
            {
                // Determine the new versions before deleting anything, so a failure leaves the current mapping intact
                var versies = await GetNewestVersiePerSelectielijstJaarAsync(newOzZaaktype, currentMapping.Id);

                await RemoveVersiesWithMappingsAsync(currentMapping.Id);
                context.ZaaktypeVersieMappings.AddRange(versies);
            }

            currentMapping.OzZaaktypeId = newOzZaaktypeId;
            currentMapping.OzZaaktypeIdentificatie = newOzZaaktype.Identificatie;
            currentMapping.OzCatalogusId = newCatalogusId;

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Takes the published versions of the OZ zaaktype and keeps the newest version per selectielijst year.
        /// Versions without a selectielijst procestype are ignored, because no selectielijst year can be derived from them.
        /// </summary>
        private async Task<List<ZaaktypeVersieMapping>> GetNewestVersiePerSelectielijstJaarAsync(OzZaaktype ozZaaktype, Guid zaaktypenMappingId)
        {
            var publishedVersies = await openZaakApiClient.GetPublishedZaaktypeVersies(ozZaaktype.Identificatie, ozZaaktype.Catalogus);

            var versiesMetJaar = new List<(int Jaar, OzZaaktype Versie)>();
            var jaarPerProcestype = new Dictionary<string, int>();

            // iterate through all versions, IGNORING those without a selectielijst procestype
            foreach (var versie in publishedVersies.Where(v => !string.IsNullOrEmpty(v.SelectielijstProcestype)))
            {
                var procestypeUrl = versie.SelectielijstProcestype!;

                if (!jaarPerProcestype.TryGetValue(procestypeUrl, out var jaar))
                {
                    var procestype = await selectielijstApiClient.GetProcestype(new Uri(procestypeUrl))
                        ?? throw new ZaaktypeVersiesException(
                            $"Het selectielijst procestype '{procestypeUrl}' van zaaktype '{ozZaaktype.Identificatie}' is niet gevonden in de Selectielijst API.");
                    jaar = procestype.Jaar;
                    jaarPerProcestype[procestypeUrl] = jaar;
                }

                versiesMetJaar.Add((jaar, versie));
            }

            if (versiesMetJaar.Count == 0)
            {
                throw new ZaaktypeVersiesException(
                    $"Zaaktype '{ozZaaktype.Identificatie}' heeft geen gepubliceerde versie met een selectielijst procestype.");
            }

            return [.. versiesMetJaar
                .GroupBy(v => v.Jaar)
                // Published versions of a zaaktype never overlap in validity, so the latest start is the newest version
                .Select(jaarGroep => jaarGroep.MaxBy(v => v.Versie.BeginGeldigheid))
                .OrderBy(v => v.Jaar)
                .Select(v => new ZaaktypeVersieMapping
                {
                    ZaaktypenMappingId = zaaktypenMappingId,
                    SelectielijstJaar = v.Jaar,
                    OzZaaktypeVersieId = v.Versie.Id
                })];
        }

        private async Task RemoveVersiesWithMappingsAsync(Guid zaaktypenMappingId)
        {
            // The database cascades these deletes, but removing them explicitly keeps the change tracker consistent
            context.StatusMappings.RemoveRange(await context.StatusMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.ResultaattypeMappings.RemoveRange(await context.ResultaattypeMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.RoltypeMappings.RemoveRange(await context.RoltypeMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.DocumenttypeMappings.RemoveRange(await context.DocumenttypeMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.BesluittypeMappings.RemoveRange(await context.BesluittypeMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.PdfInformatieobjecttypeMappings.RemoveRange(await context.PdfInformatieobjecttypeMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.VertrouwelijkheidMappings.RemoveRange(await context.VertrouwelijkheidMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.PublicatieNiveauMappings.RemoveRange(await context.PublicatieNiveauMappings.Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
            context.ZaaktypeVersieMappings.RemoveRange(await context.ZaaktypeVersieMappings.Where(v => v.ZaaktypenMappingId == zaaktypenMappingId).ToListAsync());
        }

        private async Task<OzZaaktype> GetOzZaaktypeAsync(Guid ozZaaktypeId)
        {
            return await openZaakApiClient.GetZaaktype(ozZaaktypeId)
                ?? throw new InvalidOperationException($"OpenZaak zaaktype with ID '{ozZaaktypeId}' was not found");
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

    /// <summary>
    /// The versions per selectielijst year could not be determined. The message is meant for the user.
    /// </summary>
    public class ZaaktypeVersiesException(string message) : Exception(message);
}

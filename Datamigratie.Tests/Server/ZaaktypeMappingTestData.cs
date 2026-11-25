using Datamigratie.Data;
using Datamigratie.Data.Entities;

namespace Datamigratie.Tests.Server;

public static class ZaaktypeMappingTestData
{
    /// <summary>
    /// Adds a DET to OZ zaaktype mapping with a single zaaktype version, which the per-version mappings can reference.
    /// </summary>
    /// <returns>The version; its <see cref="ZaaktypeVersieMapping.ZaaktypenMappingId"/> is the id of the zaaktype mapping</returns>
    public static ZaaktypeVersieMapping AddZaaktypenMappingWithVersie(this DatamigratieDbContext context, string? detZaaktypeId = null)
    {
        var mapping = new ZaaktypenMapping
        {
            Id = Guid.NewGuid(),
            DetZaaktypeId = detZaaktypeId ?? $"det-{Guid.NewGuid()}",
            OzZaaktypeId = Guid.NewGuid(),
            OzZaaktypeIdentificatie = "ZT-001",
            OzCatalogusId = Guid.NewGuid()
        };
        var versie = new ZaaktypeVersieMapping
        {
            Id = Guid.NewGuid(),
            ZaaktypenMappingId = mapping.Id,
            SelectielijstJaar = 2020,
            OzZaaktypeVersieId = mapping.OzZaaktypeId
        };

        context.Mappings.Add(mapping);
        context.ZaaktypeVersieMappings.Add(versie);
        context.SaveChanges();
        return versie;
    }
}

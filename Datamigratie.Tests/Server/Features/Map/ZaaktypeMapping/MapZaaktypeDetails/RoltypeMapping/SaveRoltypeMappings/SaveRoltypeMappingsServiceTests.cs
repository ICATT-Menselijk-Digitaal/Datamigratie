using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
using Datamigratie.Tests.Server;
using Microsoft.EntityFrameworkCore;
using RoltypeMappingEntity = Datamigratie.Data.Entities.RoltypeMapping;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;

public class SaveRoltypeMappingsServiceTests
{
    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static SaveRoltypeMappingsService CreateService(DatamigratieDbContext context) => new(context);

    private static SaveRoltypeMappingsRequest BuildRequest(params (string DetRol, bool AlleenPdf, string? OzRoltypeUrl)[] mappings) =>
        new()
        {
            Mappings = mappings.Select(m => new RoltypeMappingItem
            {
                DetRol = m.DetRol,
                AlleenPdf = m.AlleenPdf,
                OzRoltypeUrl = m.OzRoltypeUrl
            }).ToList()
        };

    [Fact]
    public async Task SaveRoltypeMappings_SavesNewMappings()
    {
        await using var context = CreateContext();
        var versie = context.AddZaaktypenMappingWithVersie();
        var mappingId = versie.ZaaktypenMappingId;
        var service = CreateService(context);

        var request = BuildRequest(
            ("Initiator", false, "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1"),
            ("Behandelaar", true, null)
        );

        await service.SaveRoltypeMappings(mappingId, request);

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Equal(2, saved.Count);
        Assert.Contains(saved, m => m.DetRol == "Initiator" && m.AlleenPdf == false && m.OzRoltypeUrl == "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1");
        Assert.Contains(saved, m => m.DetRol == "Behandelaar" && m.AlleenPdf == true && m.OzRoltypeUrl == null);
    }

    [Fact]
    public async Task SaveRoltypeMappings_ReplacesExistingMappings_OnReSave()
    {
        await using var context = CreateContext();
        var versie = context.AddZaaktypenMappingWithVersie();
        var mappingId = versie.ZaaktypenMappingId;

        // Seed an existing mapping
        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypeVersieMappingId = versie.Id,
            DetRol = "Initiator",
            AlleenPdf = false,
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/old-uuid"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        // Save with updated URL
        var request = BuildRequest(
            ("Initiator", false, "https://openzaak.example.com/catalogi/api/v1/roltypen/new-uuid")
        );

        await service.SaveRoltypeMappings(mappingId, request);

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Single(saved);
        Assert.Equal("https://openzaak.example.com/catalogi/api/v1/roltypen/new-uuid", saved[0].OzRoltypeUrl);
    }

    [Fact]
    public async Task SaveRoltypeMappings_RemovesAllExistingMappings_WhenRequestIsEmpty()
    {
        await using var context = CreateContext();
        var versie = context.AddZaaktypenMappingWithVersie();
        var mappingId = versie.ZaaktypenMappingId;

        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypeVersieMappingId = versie.Id,
            DetRol = "Initiator",
            AlleenPdf = false,
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.SaveRoltypeMappings(mappingId, new SaveRoltypeMappingsRequest { Mappings = [] });

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Empty(saved);
    }

    [Fact]
    public async Task SaveRoltypeMappings_OnlyAffectsMappingsForGivenZaaktypeMapping()
    {
        await using var context = CreateContext();
        var targetVersie = context.AddZaaktypenMappingWithVersie();
        var targetMappingId = targetVersie.ZaaktypenMappingId;
        var otherVersie = context.AddZaaktypenMappingWithVersie();
        var otherMappingId = otherVersie.ZaaktypenMappingId;

        // Seed a mapping for a different zaaktype
        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypeVersieMappingId = otherVersie.Id,
            DetRol = "Initiator",
            AlleenPdf = false,
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/other"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.SaveRoltypeMappings(targetMappingId, BuildRequest(
            ("Behandelaar", false, "https://openzaak.example.com/catalogi/api/v1/roltypen/target")
        ));

        // The other zaaktype's mapping should be untouched
        var otherMappings = await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == otherMappingId)
            .ToListAsync();

        Assert.Single(otherMappings);
        Assert.Equal("Initiator", otherMappings[0].DetRol);
    }
}

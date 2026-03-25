using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
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

    private static SaveRoltypeMappingsRequest BuildRequest(params (string DetRol, string OzRoltypeUrl)[] mappings) =>
        new()
        {
            Mappings = mappings.Select(m => new RoltypeMappingItem
            {
                DetRol = m.DetRol,
                OzRoltypeUrl = m.OzRoltypeUrl
            }).ToList()
        };

    [Fact]
    public async Task SaveRoltypeMappings_SavesNewMappings()
    {
        await using var context = CreateContext();
        var mappingId = Guid.NewGuid();
        var service = CreateService(context);

        var request = BuildRequest(
            ("Initiator", "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1"),
            ("Behandelaar", "alleen_pdf")
        );

        await service.SaveRoltypeMappings(mappingId, request);

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Equal(2, saved.Count);
        Assert.Contains(saved, m => m.DetRol == "Initiator" && m.OzRoltypeUrl == "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1");
        Assert.Contains(saved, m => m.DetRol == "Behandelaar" && m.OzRoltypeUrl == "alleen_pdf");
    }

    [Fact]
    public async Task SaveRoltypeMappings_ReplacesExistingMappings_OnReSave()
    {
        await using var context = CreateContext();
        var mappingId = Guid.NewGuid();

        // Seed an existing mapping
        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypenMappingId = mappingId,
            DetRol = "Initiator",
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/old-uuid"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        // Save with updated URL
        var request = BuildRequest(
            ("Initiator", "https://openzaak.example.com/catalogi/api/v1/roltypen/new-uuid")
        );

        await service.SaveRoltypeMappings(mappingId, request);

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Single(saved);
        Assert.Equal("https://openzaak.example.com/catalogi/api/v1/roltypen/new-uuid", saved[0].OzRoltypeUrl);
    }

    [Fact]
    public async Task SaveRoltypeMappings_RemovesAllExistingMappings_WhenRequestIsEmpty()
    {
        await using var context = CreateContext();
        var mappingId = Guid.NewGuid();

        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypenMappingId = mappingId,
            DetRol = "Initiator",
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.SaveRoltypeMappings(mappingId, new SaveRoltypeMappingsRequest { Mappings = [] });

        var saved = await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == mappingId)
            .ToListAsync();

        Assert.Empty(saved);
    }

    [Fact]
    public async Task SaveRoltypeMappings_OnlyAffectsMappingsForGivenZaaktypeMapping()
    {
        await using var context = CreateContext();
        var targetMappingId = Guid.NewGuid();
        var otherMappingId = Guid.NewGuid();

        // Seed a mapping for a different zaaktype
        context.RoltypeMappings.Add(new RoltypeMappingEntity
        {
            ZaaktypenMappingId = otherMappingId,
            DetRol = "Initiator",
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/other"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        await service.SaveRoltypeMappings(targetMappingId, BuildRequest(
            ("Behandelaar", "https://openzaak.example.com/catalogi/api/v1/roltypen/target")
        ));

        // The other zaaktype's mapping should be untouched
        var otherMappings = await context.RoltypeMappings
            .Where(m => m.ZaaktypenMappingId == otherMappingId)
            .ToListAsync();

        Assert.Single(otherMappings);
        Assert.Equal("Initiator", otherMappings[0].DetRol);
    }
}

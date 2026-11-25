using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;
using Datamigratie.Tests.Server;
using Microsoft.EntityFrameworkCore;
using RoltypeMappingEntity = Datamigratie.Data.Entities.RoltypeMapping;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.ShowRoltypeMappings;

public class ShowRoltypeMappingsServiceTests
{
    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static ShowRoltypeMappingsService CreateService(DatamigratieDbContext context) => new(context);

    [Fact]
    public async Task GetRoltypeMappings_ReturnsEmptyList_WhenNoMappingsExist()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.GetRoltypeMappings(Guid.NewGuid());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRoltypeMappings_ReturnsMappings_ForGivenZaaktypeVersie()
    {
        await using var context = CreateContext();
        var versie = context.AddZaaktypenMappingWithVersie();

        context.RoltypeMappings.AddRange(
            new RoltypeMappingEntity
            {
                ZaaktypeVersieMappingId = versie.Id,
                DetRol = "Initiator",
                AlleenPdf = false,
                OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1"
            },
            new RoltypeMappingEntity
            {
                ZaaktypeVersieMappingId = versie.Id,
                DetRol = "Behandelaar",
                AlleenPdf = true,
                OzRoltypeUrl = null
            }
        );
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetRoltypeMappings(versie.Id);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, m => m.DetRol == "Initiator" && m.AlleenPdf == false && m.OzRoltypeUrl == "https://openzaak.example.com/catalogi/api/v1/roltypen/uuid-1");
        Assert.Contains(result, m => m.DetRol == "Behandelaar" && m.AlleenPdf == true && m.OzRoltypeUrl == null);
    }

    [Fact]
    public async Task GetRoltypeMappings_OnlyReturnsMappings_ForGivenZaaktypeVersie()
    {
        await using var context = CreateContext();
        var targetVersie = context.AddZaaktypenMappingWithVersie();
        var otherVersie = context.AddVersie(targetVersie.ZaaktypenMappingId, 2017);

        context.RoltypeMappings.AddRange(
            new RoltypeMappingEntity
            {
                ZaaktypeVersieMappingId = targetVersie.Id,
                DetRol = "Initiator",
                AlleenPdf = false,
                OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/target"
            },
            new RoltypeMappingEntity
            {
                ZaaktypeVersieMappingId = otherVersie.Id,
                DetRol = "Behandelaar",
                AlleenPdf = false,
                OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/other"
            }
        );
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetRoltypeMappings(targetVersie.Id);

        Assert.Single(result);
        Assert.Equal("Initiator", result[0].DetRol);
    }
}

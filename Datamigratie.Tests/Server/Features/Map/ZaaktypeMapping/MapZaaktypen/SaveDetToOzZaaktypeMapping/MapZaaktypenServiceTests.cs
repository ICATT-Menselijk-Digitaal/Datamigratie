using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;

public class MapZaaktypenServiceTests
{
    private const string DetZaaktypeId = "zaaktype-001";

    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static MapZaaktypenService CreateService(DatamigratieDbContext context)
    {
        var openZaak = new Mock<IOpenZaakApiClient>();
        openZaak.Setup(c => c.GetZaaktype(It.IsAny<Guid>())).ReturnsAsync((Guid id) => new OzZaaktype
        {
            Url = $"https://openzaak.example.com/catalogi/api/v1/zaaktypen/{id}",
            Identificatie = "ZT-001",
            Omschrijving = "Test zaaktype"
        });

        var det = new Mock<IDetApiClient>();
        det.Setup(c => c.GetZaaktype(It.IsAny<string>())).ReturnsAsync((string id) => new DetZaaktype
        {
            Naam = id,
            Omschrijving = "Test zaaktype",
            FunctioneleIdentificatie = id
        });

        return new MapZaaktypenService(openZaak.Object, det.Object, context);
    }

    [Fact]
    public async Task CreateZaaktypenMapping_CreatesOneVersion_ForTheChosenOzZaaktype()
    {
        await using var context = CreateContext();
        var ozZaaktypeId = Guid.NewGuid();

        await CreateService(context).CreateZaaktypenMapping(DetZaaktypeId, ozZaaktypeId);

        var mapping = await context.Mappings.SingleAsync();
        var versie = await context.ZaaktypeVersieMappings.SingleAsync();
        Assert.Equal(mapping.Id, versie.ZaaktypenMappingId);
        Assert.Equal(ozZaaktypeId, versie.OzZaaktypeVersieId);
        Assert.Null(versie.SelectielijstJaar);
    }

    [Fact]
    public async Task UpdateZaaktypenMapping_RepointsVersion_AndKeepsRoltypeMappings()
    {
        await using var context = CreateContext();
        var service = CreateService(context);
        await service.CreateZaaktypenMapping(DetZaaktypeId, Guid.NewGuid());
        var versie = await context.ZaaktypeVersieMappings.SingleAsync();
        versie.SelectielijstJaar = 2017;
        context.RoltypeMappings.Add(new RoltypeMapping { ZaaktypeVersieMappingId = versie.Id, DetRol = "Initiator", AlleenPdf = true });
        context.StatusMappings.Add(new StatusMapping { ZaaktypeVersieMappingId = versie.Id, DetStatusNaam = "Afgerond", OzStatustypeId = Guid.NewGuid() });
        await context.SaveChangesAsync();
        var newOzZaaktypeId = Guid.NewGuid();

        await service.UpdateZaaktypenMapping(DetZaaktypeId, newOzZaaktypeId);

        var updated = await context.ZaaktypeVersieMappings.SingleAsync();
        Assert.Equal(versie.Id, updated.Id);
        Assert.Equal(newOzZaaktypeId, updated.OzZaaktypeVersieId);
        Assert.Null(updated.SelectielijstJaar);
        Assert.Single(await context.RoltypeMappings.ToListAsync());
        Assert.Empty(await context.StatusMappings.ToListAsync());
    }
}

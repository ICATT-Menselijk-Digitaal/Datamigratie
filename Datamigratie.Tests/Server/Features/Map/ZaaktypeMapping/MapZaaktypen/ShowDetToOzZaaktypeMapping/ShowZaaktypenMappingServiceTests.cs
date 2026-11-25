using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowDetToOzZaaktypeMapping;

public class ShowZaaktypenMappingServiceTests
{
    private const string DetZaaktypeId = "zaaktype-001";

    private readonly DatamigratieDbContext _context = new(new DbContextOptionsBuilder<DatamigratieDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    [Fact]
    public async Task GetZaaktypenMapping_ReturnsNull_WhenNoMappingExists()
    {
        var result = await new ShowZaaktypenMappingService(_context).GetZaaktypenMapping(DetZaaktypeId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetZaaktypenMapping_ReturnsVersionsOrderedBySelectielijstJaar()
    {
        var versie2020 = _context.AddZaaktypenMappingWithVersie(DetZaaktypeId);
        var versie2017 = _context.AddVersie(versie2020.ZaaktypenMappingId, 2017);
        _context.AddZaaktypenMappingWithVersie("other-zaaktype");

        var result = await new ShowZaaktypenMappingService(_context).GetZaaktypenMapping(DetZaaktypeId);

        Assert.NotNull(result);
        Assert.Equal(versie2020.ZaaktypenMappingId, result.Id);
        Assert.Equal([versie2017.Id, versie2020.Id], result.Versies.Select(v => v.Id));
        Assert.Equal([2017, 2020], result.Versies.Select(v => v.SelectielijstJaar));
        Assert.Equal(versie2017.OzZaaktypeVersieId, result.Versies[0].OzZaaktypeVersieId);
    }
}

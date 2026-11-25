using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Common.Services.Selectielijst;
using Datamigratie.Common.Services.Selectielijst.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.SaveDetToOzZaaktypeMapping;

public class MapZaaktypenServiceTests
{
    private const string DetZaaktypeId = "zaaktype-001";
    private const string OzBaseUrl = "https://openzaak.example.com/catalogi/api/v1";
    private const string SelectielijstBaseUrl = "https://selectielijst.example.com/api/v1/procestypen";

    private static readonly Guid CatalogusId = Guid.NewGuid();
    private static readonly string Procestype2017 = $"{SelectielijstBaseUrl}/{Guid.NewGuid()}";
    private static readonly string Procestype2020 = $"{SelectielijstBaseUrl}/{Guid.NewGuid()}";

    private readonly Mock<IOpenZaakApiClient> _openZaak = new();
    private readonly Mock<ISelectielijstApiClient> _selectielijst = new();
    private readonly DatamigratieDbContext _context = new(new DbContextOptionsBuilder<DatamigratieDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);

    public MapZaaktypenServiceTests()
    {
        _selectielijst.Setup(c => c.GetProcestype(new Uri(Procestype2017), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Procestype(Procestype2017, 2017));
        _selectielijst.Setup(c => c.GetProcestype(new Uri(Procestype2020), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Procestype(Procestype2020, 2020));
    }

    private MapZaaktypenService CreateService()
    {
        var det = new Mock<IDetApiClient>();
        det.Setup(c => c.GetZaaktype(It.IsAny<string>())).ReturnsAsync((string id) => new DetZaaktype
        {
            Naam = id,
            Omschrijving = "Test zaaktype",
            FunctioneleIdentificatie = id
        });

        return new MapZaaktypenService(_openZaak.Object, det.Object, _selectielijst.Object, _context);
    }

    private static SelectielijstProcestype Procestype(string url, int jaar) => new()
    {
        Url = url,
        Nummer = 6,
        Jaar = jaar,
        Naam = "Toezicht houden"
    };

    private static OzZaaktype Versie(string identificatie, string? procestype, DateOnly beginGeldigheid, Guid? catalogusId = null) => new()
    {
        Url = $"{OzBaseUrl}/zaaktypen/{Guid.NewGuid()}",
        Identificatie = identificatie,
        Omschrijving = "Test zaaktype",
        Catalogus = $"{OzBaseUrl}/catalogussen/{catalogusId ?? CatalogusId}",
        SelectielijstProcestype = procestype,
        Versiedatum = beginGeldigheid,
        BeginGeldigheid = beginGeldigheid
    };

    /// <summary>
    /// Makes the versions known to the OpenZaak mock: each one by its id, and all of them as the published versions of their zaaktype.
    /// </summary>
    private void SetupOzZaaktype(params OzZaaktype[] versies)
    {
        foreach (var versie in versies)
        {
            _openZaak.Setup(c => c.GetZaaktype(versie.Id)).ReturnsAsync(versie);
        }

        _openZaak.Setup(c => c.GetPublishedZaaktypeVersies(versies[0].Identificatie, versies[0].Catalogus))
            .ReturnsAsync([.. versies]);
    }

    private async Task<ZaaktypeVersieMapping> AddRoltypeMappingAsync()
    {
        var versie = await _context.ZaaktypeVersieMappings.FirstAsync();
        _context.RoltypeMappings.Add(new RoltypeMapping { ZaaktypeVersieMappingId = versie.Id, DetRol = "Initiator", AlleenPdf = true });
        await _context.SaveChangesAsync();
        return versie;
    }

    [Fact]
    public async Task CreateZaaktypenMapping_CreatesNewestVersionPerSelectielijstJaar()
    {
        var old2017 = Versie("ZT-001", Procestype2017, new DateOnly(2018, 1, 1));
        var new2017 = Versie("ZT-001", Procestype2017, new DateOnly(2019, 1, 1));
        var only2020 = Versie("ZT-001", Procestype2020, new DateOnly(2021, 1, 1));
        SetupOzZaaktype(old2017, new2017, only2020);

        await CreateService().CreateZaaktypenMapping(DetZaaktypeId, old2017.Id);

        var mapping = await _context.Mappings.SingleAsync();
        Assert.Equal("ZT-001", mapping.OzZaaktypeIdentificatie);
        Assert.Equal(CatalogusId, mapping.OzCatalogusId);
        var versies = await _context.ZaaktypeVersieMappings.OrderBy(v => v.SelectielijstJaar).ToListAsync();
        Assert.Equal([2017, 2020], versies.Select(v => v.SelectielijstJaar));
        Assert.Equal([new2017.Id, only2020.Id], versies.Select(v => v.OzZaaktypeVersieId));
        Assert.All(versies, v => Assert.Equal(mapping.Id, v.ZaaktypenMappingId));
    }

    [Fact]
    public async Task CreateZaaktypenMapping_IgnoresVersionsWithoutSelectielijstProcestype()
    {
        var withoutProcestype = Versie("ZT-001", null, new DateOnly(2022, 1, 1));
        var with2020 = Versie("ZT-001", Procestype2020, new DateOnly(2021, 1, 1));
        SetupOzZaaktype(withoutProcestype, with2020);

        await CreateService().CreateZaaktypenMapping(DetZaaktypeId, withoutProcestype.Id);

        var versie = await _context.ZaaktypeVersieMappings.SingleAsync();
        Assert.Equal(2020, versie.SelectielijstJaar);
        Assert.Equal(with2020.Id, versie.OzZaaktypeVersieId);
    }

    [Fact]
    public async Task CreateZaaktypenMapping_Throws_WhenNoVersionHasASelectielijstProcestype()
    {
        var versie = Versie("ZT-001", null, new DateOnly(2022, 1, 1));
        SetupOzZaaktype(versie);

        await Assert.ThrowsAsync<ZaaktypeVersiesException>(() => CreateService().CreateZaaktypenMapping(DetZaaktypeId, versie.Id));

        Assert.Empty(await _context.Mappings.ToListAsync());
    }

    [Fact]
    public async Task CreateZaaktypenMapping_Throws_WhenSelectielijstProcestypeIsUnknown()
    {
        var versie = Versie("ZT-001", $"{SelectielijstBaseUrl}/{Guid.NewGuid()}", new DateOnly(2022, 1, 1));
        SetupOzZaaktype(versie);

        await Assert.ThrowsAsync<ZaaktypeVersiesException>(() => CreateService().CreateZaaktypenMapping(DetZaaktypeId, versie.Id));

        Assert.Empty(await _context.Mappings.ToListAsync());
    }

    [Fact]
    public async Task UpdateZaaktypenMapping_ToOtherZaaktype_ReplacesVersionsAndDeletesTheirMappings()
    {
        var current = Versie("ZT-001", Procestype2017, new DateOnly(2018, 1, 1));
        var other = Versie("ZT-002", Procestype2020, new DateOnly(2021, 1, 1));
        SetupOzZaaktype(current);
        SetupOzZaaktype(other);
        var service = CreateService();
        await service.CreateZaaktypenMapping(DetZaaktypeId, current.Id);
        await AddRoltypeMappingAsync();

        await service.UpdateZaaktypenMapping(DetZaaktypeId, other.Id);

        var mapping = await _context.Mappings.SingleAsync();
        Assert.Equal(other.Id, mapping.OzZaaktypeId);
        Assert.Equal("ZT-002", mapping.OzZaaktypeIdentificatie);
        var versie = await _context.ZaaktypeVersieMappings.SingleAsync();
        Assert.Equal(2020, versie.SelectielijstJaar);
        Assert.Equal(other.Id, versie.OzZaaktypeVersieId);
        Assert.Empty(await _context.RoltypeMappings.ToListAsync());
    }

    [Fact]
    public async Task UpdateZaaktypenMapping_ToOtherVersionOfSameZaaktype_KeepsVersionsAndMappings()
    {
        var older = Versie("ZT-001", Procestype2017, new DateOnly(2018, 1, 1));
        var newer = Versie("ZT-001", Procestype2017, new DateOnly(2019, 1, 1));
        SetupOzZaaktype(older, newer);
        var service = CreateService();
        await service.CreateZaaktypenMapping(DetZaaktypeId, older.Id);
        var versie = await AddRoltypeMappingAsync();

        await service.UpdateZaaktypenMapping(DetZaaktypeId, newer.Id);

        Assert.Equal(newer.Id, (await _context.Mappings.SingleAsync()).OzZaaktypeId);
        Assert.Equal(versie.Id, (await _context.ZaaktypeVersieMappings.SingleAsync()).Id);
        Assert.Single(await _context.RoltypeMappings.ToListAsync());
    }

    [Fact]
    public async Task UpdateZaaktypenMapping_KeepsCurrentMapping_WhenNewZaaktypeHasNoVersionWithSelectielijstProcestype()
    {
        var current = Versie("ZT-001", Procestype2017, new DateOnly(2018, 1, 1));
        var other = Versie("ZT-002", null, new DateOnly(2021, 1, 1));
        SetupOzZaaktype(current);
        SetupOzZaaktype(other);
        var service = CreateService();
        await service.CreateZaaktypenMapping(DetZaaktypeId, current.Id);
        await AddRoltypeMappingAsync();

        await Assert.ThrowsAsync<ZaaktypeVersiesException>(() => service.UpdateZaaktypenMapping(DetZaaktypeId, other.Id));

        Assert.Equal(current.Id, (await _context.Mappings.SingleAsync()).OzZaaktypeId);
        Assert.Equal(current.Id, (await _context.ZaaktypeVersieMappings.SingleAsync()).OzZaaktypeVersieId);
        Assert.Single(await _context.RoltypeMappings.ToListAsync());
    }
}

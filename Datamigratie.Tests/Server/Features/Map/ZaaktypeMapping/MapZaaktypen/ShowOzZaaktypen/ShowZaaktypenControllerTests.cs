using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen.Models;
using Moq;

namespace Datamigratie.Tests.Server.Features.Map.ZaaktypeMapping.MapZaaktypen.ShowOzZaaktypen;

public class ShowZaaktypenControllerTests
{
    private const string OzBaseUrl = "https://openzaak.example.com/catalogi/api/v1";

    private static OzZaaktype Versie(string identificatie, Guid catalogusId, DateOnly beginGeldigheid, string omschrijving = "Test zaaktype") => new()
    {
        Url = $"{OzBaseUrl}/zaaktypen/{Guid.NewGuid()}",
        Identificatie = identificatie,
        Omschrijving = omschrijving,
        Catalogus = $"{OzBaseUrl}/catalogussen/{catalogusId}",
        Versiedatum = beginGeldigheid,
        BeginGeldigheid = beginGeldigheid
    };

    private static async Task<List<OzZaaktypeOptie>> GetOpties(params OzZaaktype[] zaaktypen)
    {
        var openZaak = new Mock<IOpenZaakApiClient>();
        openZaak.Setup(c => c.GetAllZaakTypen()).ReturnsAsync([.. zaaktypen]);
        return [.. await new ShowZaaktypenController(openZaak.Object).GetAllZaakTypen()];
    }

    [Fact]
    public async Task GetAllZaakTypen_ReturnsEachZaaktypeOnce_RepresentedByItsNewestVersion()
    {
        var catalogusId = Guid.NewGuid();
        var newest = Versie("ZT-001", catalogusId, new DateOnly(2021, 1, 1), "Nieuwste omschrijving");
        var older = Versie("ZT-001", catalogusId, new DateOnly(2018, 1, 1), "Oude omschrijving");

        var opties = await GetOpties(older, newest);

        var optie = Assert.Single(opties);
        Assert.Equal("ZT-001", optie.Identificatie);
        Assert.Equal("Nieuwste omschrijving", optie.Omschrijving);
        Assert.Equal(catalogusId, optie.CatalogusId);
    }

    [Fact]
    public async Task GetAllZaakTypen_KeepsSameIdentificatieInDifferentCatalogi_Apart()
    {
        var inCatalogusA = Versie("ZT-001", Guid.NewGuid(), new DateOnly(2021, 1, 1));
        var inCatalogusB = Versie("ZT-001", Guid.NewGuid(), new DateOnly(2021, 1, 1));

        var opties = await GetOpties(inCatalogusA, inCatalogusB);

        Assert.Equal(2, opties.Count);
    }
}

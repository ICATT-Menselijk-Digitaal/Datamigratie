using System.Net;
using Datamigratie.Common.Services.OpenZaak;
using Microsoft.Extensions.Logging.Abstractions;

namespace Datamigratie.Tests.Common.OpenZaakApiClientTests;

public class GetPublishedZaaktypeVersiesTests
{
    private const string CatalogusUrl = "https://openzaak.example.com/catalogi/api/v1/catalogussen/9b1d3c4f-2a4e-4c4a-8f5e-1e2d3c4b5a69";

    private const string TwoVersionsResponse = $$"""
        {
          "count": 2, "next": null, "previous": null,
          "results": [
            {
              "url": "https://openzaak.example.com/catalogi/api/v1/zaaktypen/0b6ba0b3-3d1f-4b55-8c5e-5a4f5e1d2c01",
              "identificatie": "TestZAAK", "omschrijving": "Test zaak", "catalogus": "{{CatalogusUrl}}",
              "selectielijstProcestype": "https://selectielijst.openzaak.nl/api/v1/procestypen/aa8aa2fd-b9c6-4e34-9a6c-58a677f60ea0",
              "versiedatum": "2026-03-12", "beginGeldigheid": "2026-03-12", "eindeGeldigheid": "2026-04-08", "concept": false
            },
            {
              "url": "https://openzaak.example.com/catalogi/api/v1/zaaktypen/0b6ba0b3-3d1f-4b55-8c5e-5a4f5e1d2c02",
              "identificatie": "TestZAAK", "omschrijving": "Test zaak", "catalogus": "{{CatalogusUrl}}",
              "selectielijstProcestype": "https://selectielijst.openzaak.nl/api/v1/procestypen/035d5ce8-d4f8-4f23-a386-fe2ea136d9e3",
              "versiedatum": "2026-04-09", "beginGeldigheid": "2026-04-09", "eindeGeldigheid": null, "concept": false
            }
          ]
        }
        """;

    private static OpenZaakClient CreateClient(StubHttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://openzaak.example.com/") }, NullLogger<OpenZaakClient>.Instance);

    [Fact]
    public async Task FiltersOnIdentificatieCatalogusAndPublishedStatus()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.OK, TwoVersionsResponse);

        await CreateClient(handler).GetPublishedZaaktypeVersies("Test ZAAK&1", CatalogusUrl);

        var requested = Assert.Single(handler.RequestedUris);
        Assert.Equal("/catalogi/api/v1/zaaktypen", requested.AbsolutePath);
        var query = System.Web.HttpUtility.ParseQueryString(requested.Query);
        Assert.Equal("Test ZAAK&1", query["identificatie"]);
        Assert.Equal(CatalogusUrl, query["catalogus"]);
        Assert.Equal("definitief", query["status"]);
        Assert.Equal("1", query["page"]);
    }

    [Fact]
    public async Task MapsVersionFields()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.OK, TwoVersionsResponse);

        var versies = await CreateClient(handler).GetPublishedZaaktypeVersies("TestZAAK", CatalogusUrl);

        Assert.Equal(2, versies.Count);
        var first = versies[0];
        Assert.Equal(Guid.Parse("0b6ba0b3-3d1f-4b55-8c5e-5a4f5e1d2c01"), first.Id);
        Assert.Equal(CatalogusUrl, first.Catalogus);
        Assert.Equal("https://selectielijst.openzaak.nl/api/v1/procestypen/aa8aa2fd-b9c6-4e34-9a6c-58a677f60ea0", first.SelectielijstProcestype);
        Assert.Equal(new DateOnly(2026, 3, 12), first.Versiedatum);
        Assert.Equal(new DateOnly(2026, 3, 12), first.BeginGeldigheid);
        Assert.Equal(new DateOnly(2026, 4, 8), first.EindeGeldigheid);
        Assert.False(first.Concept);
        Assert.Null(versies[1].EindeGeldigheid);
    }
}

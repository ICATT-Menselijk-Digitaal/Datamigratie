using System.Net;
using Datamigratie.Common.Services.Selectielijst;

namespace Datamigratie.Tests.Common.Services.Selectielijst;

public class SelectielijstApiClientTests
{
    private const string ProcestypeUrl = "https://selectielijst.openzaak.nl/api/v1/procestypen/035d5ce8-d4f8-4f23-a386-fe2ea136d9e3";
    private const string ResultaatUrl = "https://selectielijst.openzaak.nl/api/v1/resultaten/b58eb4af-95a0-4148-907b-3732def8c751";

    private static SelectielijstApiClient CreateClient(StubHttpMessageHandler handler) => new(new HttpClient(handler));

    [Fact]
    public async Task GetProcestype_ReturnsNummerAndJaar_FromAbsoluteUrl()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.OK, $$"""
            {"url":"{{ProcestypeUrl}}","nummer":11,"jaar":2017,"naam":"Toestemming verlenen","omschrijving":"","toelichting":"","procesobject":"De verleende toestemming"}
            """);

        var procestype = await CreateClient(handler).GetProcestype(new Uri(ProcestypeUrl));

        Assert.NotNull(procestype);
        Assert.Equal(11, procestype.Nummer);
        Assert.Equal(2017, procestype.Jaar);
        Assert.Equal(ProcestypeUrl, procestype.Url);
        Assert.Equal(new Uri(ProcestypeUrl), Assert.Single(handler.RequestedUris));
    }

    [Fact]
    public async Task GetResultaat_ReturnsVolledigNummer()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.OK, $$"""
            {"url":"{{ResultaatUrl}}","procesType":"{{ProcestypeUrl}}","nummer":1,"volledigNummer":"11.1","generiek":true,"naam":"Verleend","waardering":"vernietigen"}
            """);

        var resultaat = await CreateClient(handler).GetResultaat(new Uri(ResultaatUrl));

        Assert.NotNull(resultaat);
        Assert.Equal("11.1", resultaat.VolledigNummer);
        Assert.Equal(ProcestypeUrl, resultaat.ProcesType);
    }

    [Fact]
    public async Task GetProcestype_ReturnsNull_WhenNotFound()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.NotFound);

        var procestype = await CreateClient(handler).GetProcestype(new Uri(ProcestypeUrl));

        Assert.Null(procestype);
    }

    [Fact]
    public async Task GetResultaat_Throws_OnServerError()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.InternalServerError);

        await Assert.ThrowsAsync<HttpRequestException>(() => CreateClient(handler).GetResultaat(new Uri(ResultaatUrl)));
    }

    [Fact]
    public async Task GetProcestype_Throws_ForRelativeUrl()
    {
        var handler = new StubHttpMessageHandler(HttpStatusCode.OK, "{}");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            CreateClient(handler).GetProcestype(new Uri("procestypen/123", UriKind.Relative)));
        Assert.Empty(handler.RequestedUris);
    }
}

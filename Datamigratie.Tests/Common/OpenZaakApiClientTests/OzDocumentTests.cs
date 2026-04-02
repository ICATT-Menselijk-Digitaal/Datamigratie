using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Tests.Common.OpenZaakApiClientTests;
public class OzDocumentTests
{
    [Theory]
    [InlineData("rapport.pdf", "rapport.pdf")]
    [InlineData("rapport (definitief).pdf", "rapport (definitief).pdf")]
    [InlineData("verslag café.pdf", "verslag café.pdf")]
    [InlineData("bëschikking ñoña.pdf", "bëschikking ñoña.pdf")]
    [InlineData("document \"v2\".pdf", "document _v2_.pdf")]
    [InlineData("path\\file.pdf", "path_file.pdf")]
    [InlineData("verzoek_om_aanvullende\ninformatie.txt", "verzoek_om_aanvullende_informatie.txt")]
    [InlineData("verzoek_om_aanvullende\r\ninformatie.txt", "verzoek_om_aanvullende__informatie.txt")]
    [InlineData("\n\n\n.pdf", "___.pdf")]
    public void Bestandsnaam_SanitizesProblematicCharacters(string input, string expected)
    {
        var document = CreateDocument(bestandsnaam: input);
        Assert.Equal(expected, document.Bestandsnaam);
    }

    [Fact]
    public void Bestandsnaam_Null_RemainsNull()
    {
        var document = CreateDocument(bestandsnaam: null);
        Assert.Null(document.Bestandsnaam);
    }

    [Fact]
    public void Bestandsnaam_WithoutProblematicCharacters_IsUnchanged()
    {
        var document = CreateDocument(bestandsnaam: "gewoon_document.pdf");
        Assert.Equal("gewoon_document.pdf", document.Bestandsnaam);
    }

    private static OzDocument CreateDocument(string? bestandsnaam) => new()
    {
        Bestandsnaam = bestandsnaam,
        Bronorganisatie = "123456789",
        Creatiedatum = new DateOnly(2025, 1, 1),
        Titel = "Test",
        Vertrouwelijkheidaanduiding = DocumentVertrouwelijkheidaanduiding.openbaar,
        Auteur = "Test",
        Status = DocumentStatus.definitief,
        Taal = "dut",
        Link = "",
        Beschrijving = "",
        Verschijningsvorm = "",
        Informatieobjecttype = new Uri("https://example.com/informatieobjecttypen/1"),
        Trefwoorden = []
    };
}

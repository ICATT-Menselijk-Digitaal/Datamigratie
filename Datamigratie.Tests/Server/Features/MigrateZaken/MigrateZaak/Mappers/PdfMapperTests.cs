using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class PdfMapperTests
{
    private const string OpenZaakBaseUrl = "https://openzaak.example.com/";
    private const string Rsin = "123456782";
    private static readonly Guid PdfInfoObjectTypeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Uri InformatieobjecttypeUri = new($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{PdfInfoObjectTypeId}");

    [Fact]
    public void Map_SetsCorrectMetadata()
    {
        var mapper = new PdfMapper(Rsin, InformatieobjecttypeUri);
        var detZaak = CreateMinimalDetZaak(identificatie: "ZAAK-PDF-001");

        var result = mapper.Map(detZaak);

        Assert.Equal("zaakgegevens_ZAAK-PDF-001.pdf", result.Bestandsnaam);
        Assert.Equal("zaakgegevens-ZAAK-PDF-001", result.Identificatie);
        Assert.Equal("application/pdf", result.Formaat);
        Assert.Equal("dut", result.Taal);
        Assert.Equal("e-Suite zaakgegevens ZAAK-PDF-001", result.Titel);
        Assert.Equal(DocumentVertrouwelijkheidaanduiding.openbaar, result.Vertrouwelijkheidaanduiding);
        Assert.Equal(DocumentStatus.definitief, result.Status);
        Assert.Equal(Rsin, result.Bronorganisatie);
        Assert.Equal(0, result.Bestandsomvang);
        Assert.Equal($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{PdfInfoObjectTypeId}", result.Informatieobjecttype.ToString());
    }

    private static DetZaak CreateMinimalDetZaak(string identificatie = "ZAAK-001")
    {
        return new DetZaak
        {
            FunctioneleIdentificatie = identificatie,
            Omschrijving = "Test",
            CreatieDatumTijd = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            Historie = []
        };
    }
}

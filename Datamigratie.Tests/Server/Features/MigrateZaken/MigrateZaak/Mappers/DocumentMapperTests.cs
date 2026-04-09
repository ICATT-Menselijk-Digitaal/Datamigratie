using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class DocumentMapperTests
{
    private const string OpenZaakBaseUrl = "https://openzaak.example.com/";
    private const string Rsin = "123456782";

    private static DocumentMapper CreateMapper(
        Dictionary<string, DocumentStatus>? documentStatusMappings = null,
        Dictionary<string, DocumentVertrouwelijkheidaanduiding>? publicatieNiveauMappings = null,
        Dictionary<string, Uri>? documenttypeMappings = null)
    {
        return new DocumentMapper(
            Rsin,
            documentStatusMappings ?? new Dictionary<string, DocumentStatus> { { "Definitief", DocumentStatus.definitief } },
            publicatieNiveauMappings ?? new Dictionary<string, DocumentVertrouwelijkheidaanduiding> { { "Publiek", DocumentVertrouwelijkheidaanduiding.openbaar } },
            documenttypeMappings ?? new Dictionary<string, Uri> { { "Brief", new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{Guid.NewGuid()}") } });
    }

    [Fact]
    public void Map_ValidInput_MapsDocumentFields()
    {
        var mapper = CreateMapper();
        var versie = CreateMinimalDetDocumentVersie(
            bestandsnaam: "test.pdf",
            mimetype: "application/pdf",
            auteur: "Jan",
            documentgrootte: 1024);

        var document = CreateMinimalDetDocument(
            titel: "Test Document",
            kenmerk: "DOC-001",
            beschrijving: "Een test",
            versies: [versie]);

        var plan = mapper.Map(document);
        var result = plan.Versions[0].Document;

        Assert.Equal("test.pdf", result.Bestandsnaam);
        Assert.Equal("application/pdf", result.Formaat);
        Assert.Equal("Jan", result.Auteur);
        Assert.Equal(1024, result.Bestandsomvang);
        Assert.Equal("DOC-001", result.Identificatie);
        Assert.Equal("Test Document", result.Titel);
        Assert.Equal("Een test", result.Beschrijving);
        Assert.Equal(Rsin, result.Bronorganisatie);
    }

    [Fact]
    public void Map_KenmerkTooLong_Throws()
    {
        var mapper = CreateMapper();
        var document = CreateMinimalDetDocument(kenmerk: new string('K', 41));

        Assert.Throws<InvalidDataException>(() =>
            mapper.Map(document));
    }

    [Fact]
    public void Map_MultipleVersions_ReturnsAllVersionPlans()
    {
        var mapper = CreateMapper();
        var document = CreateMinimalDetDocument(versies:
        [
            CreateMinimalDetDocumentVersie(versienummer: 1, documentInhoudId: 100),
            CreateMinimalDetDocumentVersie(versienummer: 2, documentInhoudId: 200),
            CreateMinimalDetDocumentVersie(versienummer: 3, documentInhoudId: 300),
        ]);

        var plan = mapper.Map(document);

        Assert.Equal(3, plan.Versions.Count);
        Assert.Equal(100, plan.Versions[0].DocumentInhoudId);
        Assert.Equal(200, plan.Versions[1].DocumentInhoudId);
        Assert.Equal(300, plan.Versions[2].DocumentInhoudId);
    }

    [Fact]
    public void Map_MapsInformatieobjecttypeUri()
    {
        var expectedUri = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var mapper = CreateMapper(documenttypeMappings: new Dictionary<string, Uri> { { "Brief", expectedUri } });

        var document = CreateMinimalDetDocument();

        var plan = mapper.Map(document);
        var result = plan.Versions[0].Document;

        Assert.Equal(expectedUri, result.Informatieobjecttype);
    }

    private static DetDocument CreateMinimalDetDocument(
        string titel = "Test Doc",
        string? kenmerk = null,
        string? beschrijving = null,
        string documentstatusNaam = "Definitief",
        string publicatieniveau = "Publiek",
        string documenttypeNaam = "Brief",
        List<DetDocumentVersie>? versies = null)
    {
        return new DetDocument
        {
            Titel = titel,
            Kenmerk = kenmerk,
            Beschrijving = beschrijving,
            Documentstatus = new DetDocumentstatus { Naam = documentstatusNaam },
            Publicatieniveau = publicatieniveau,
            Documenttype = new DetDocumenttype { Naam = documenttypeNaam },
            DocumentVersies = versies ?? [CreateMinimalDetDocumentVersie()],
            AanvraagDocument = false,
            Historie = []
        };
    }

    private static DetDocumentVersie CreateMinimalDetDocumentVersie(
        int versienummer = 1,
        long documentInhoudId = 100,
        string bestandsnaam = "test.pdf",
        string mimetype = "application/pdf",
        string? auteur = "Auteur",
        long? documentgrootte = 500)
    {
        return new DetDocumentVersie
        {
            Versienummer = versienummer,
            DocumentInhoudID = documentInhoudId,
            Bestandsnaam = bestandsnaam,
            Mimetype = mimetype,
            Compressed = false,
            Auteur = auteur,
            Documentgrootte = documentgrootte,
            Creatiedatum = new DateOnly(2024, 1, 1)
        };
    }
}

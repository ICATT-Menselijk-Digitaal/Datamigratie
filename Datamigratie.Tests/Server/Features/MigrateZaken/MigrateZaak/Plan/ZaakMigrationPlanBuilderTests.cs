using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Plan;

public class ZaakMigrationPlanBuilderTests
{
    private const string OpenZaakBaseUrl = "https://openzaak.example.com/";
    private const string Rsin = "123456782";
    private static readonly Guid ZaaktypeId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid PdfInfoObjectTypeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    #region BuildResultaatPlan — via ResultaatMapper

    [Fact]
    public void Build_NoResultaat_ResultaatPlanIsNull()
    {
        var detZaak = CreateMinimalDetZaak();
        detZaak.Resultaat = null;
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping();
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.Null(plan.Resultaat);
    }

    [Fact]
    public void Build_ResultaatWithMapping_ResultaatPlanHasUri()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/resultaattypen/1234");
        var detZaak = CreateMinimalDetZaak();
        detZaak.Resultaat = new DetResultaat { Actief = true, Naam = "Toegekend" };
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping(
            resultaatMappings: new Dictionary<string, Uri> { { "Toegekend", uri } });
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.NotNull(plan.Resultaat);
        Assert.Equal(uri, plan.Resultaat.Resultaattype);
    }

    [Fact]
    public void Build_ResultaatWithoutMapping_ResultaatPlanIsNull()
    {
        var detZaak = CreateMinimalDetZaak();
        detZaak.Resultaat = new DetResultaat { Actief = true, Naam = "Onbekend" };
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping();
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.Null(plan.Resultaat);
    }

    #endregion

    #region BuildStatusPlan — via StatusMapper

    [Fact]
    public void Build_NoStatus_StatusPlanIsNull()
    {
        var detZaak = CreateMinimalDetZaak();
        detZaak.ZaakStatus = null;
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping();
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.Null(plan.Status);
    }

    [Fact]
    public void Build_StatusWithMapping_StatusPlanHasUriAndDate()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/statustypen/1234");
        var einddatum = new DateOnly(2024, 6, 15);
        var detZaak = CreateMinimalDetZaak(einddatum: einddatum);
        detZaak.ZaakStatus = new DetStatus { Uitwisselingscode = "", Naam = "Afgehandeld", Actief = false };
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping(
            statusMappings: new Dictionary<string, Uri> { { "Afgehandeld", uri } });
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.NotNull(plan.Status);
        Assert.Equal(uri, plan.Status.Statustype);
        Assert.Equal(einddatum.ToDateTime(TimeOnly.MinValue), plan.Status.DatumStatusGezet);
    }

    [Fact]
    public void Build_StatusWithMapping_NoEinddatum_Throws()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/statustypen/1234");
        var detZaak = CreateMinimalDetZaak(einddatum: null);
        detZaak.ZaakStatus = new DetStatus { Uitwisselingscode = "", Naam = "Afgehandeld", Actief = false };
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping(
            statusMappings: new Dictionary<string, Uri> { { "Afgehandeld", uri } });

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ZaakMigrationPlanBuilder.Build(detZaak, mapping));
        Assert.Contains("einddatum", ex.Message);
    }

    [Fact]
    public void Build_StatusWithoutMapping_StatusPlanIsNull()
    {
        var detZaak = CreateMinimalDetZaak(einddatum: new DateOnly(2024, 6, 15));
        detZaak.ZaakStatus = new DetStatus { Uitwisselingscode = "", Naam = "Onbekend", Actief = false };
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping();
        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.Null(plan.Status);
    }

    #endregion

    #region Full Build integration

    [Fact]
    public void Build_ProducesCompletePlan()
    {
        var besluitGuid = Guid.NewGuid();
        var docTypeGuid = Guid.NewGuid();
        var resultaatUri = new Uri("https://openzaak.example.com/catalogi/api/v1/resultaattypen/1234");
        var statusUri = new Uri("https://openzaak.example.com/catalogi/api/v1/statustypen/5678");

        var detZaak = CreateMinimalDetZaak(einddatum: new DateOnly(2024, 12, 31));
        detZaak.Resultaat = new DetResultaat { Actief = true, Naam = "Toegekend" };
        detZaak.ZaakStatus = new DetStatus { Uitwisselingscode = "", Naam = "Afgehandeld", Actief = false };
        detZaak.Documenten =
        [
            CreateMinimalDetDocument(versies: [CreateMinimalDetDocumentVersie()])
        ];
        detZaak.Besluiten =
        [
            new DetBesluit
            {
                FunctioneleIdentificatie = "BESLUIT-INT",
                Besluittype = new DetBesluittype { Naam = "Toekenning" },
                BesluitDatum = new DateOnly(2024, 6, 1),
            }
        ];

        var besluitUri = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/besluittypen/{besluitGuid}");
        var docTypeUri = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{docTypeGuid}");
        var mapping = CreateMinimalMapping(
            resultaatMappings: new Dictionary<string, Uri> { { "Toegekend", resultaatUri } },
            statusMappings: new Dictionary<string, Uri> { { "Afgehandeld", statusUri } },
            besluittypeMappings: new Dictionary<string, Uri> { { "Toekenning", besluitUri } },
            documenttypeMappings: new Dictionary<string, Uri> { { "Brief", docTypeUri } });

        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.NotNull(plan.ZaakRequest);
        Assert.NotNull(plan.Resultaat);
        Assert.Equal(resultaatUri, plan.Resultaat.Resultaattype);
        Assert.NotNull(plan.Status);
        Assert.Equal(statusUri, plan.Status.Statustype);
        Assert.NotNull(plan.PdfDocument);
        Assert.Single(plan.Documents);
        Assert.Single(plan.Besluiten);
    }

    [Fact]
    public void Build_NoResultaatNoStatus_PlansAreNull()
    {
        var detZaak = CreateMinimalDetZaak();
        detZaak.Resultaat = null;
        detZaak.ZaakStatus = null;
        detZaak.Documenten = [];
        detZaak.Besluiten = [];

        var mapping = CreateMinimalMapping();

        var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

        Assert.Null(plan.Resultaat);
        Assert.Null(plan.Status);
        Assert.Empty(plan.Documents);
        Assert.Empty(plan.Besluiten);
    }

    #endregion

    #region Helpers

    private static DetZaak CreateMinimalDetZaak(
        string identificatie = "ZAAK-001",
        string omschrijving = "Test",
        string? externeIdentificatie = null,
        DateOnly? einddatum = null)
    {
        return new DetZaak
        {
            FunctioneleIdentificatie = identificatie,
            Omschrijving = omschrijving,
            CreatieDatumTijd = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            ExterneIdentificatie = externeIdentificatie,
            Einddatum = einddatum,
            Historie = []
        };
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

    private static MigrateZaakMappingModel CreateMinimalMapping(
        Dictionary<string, Uri>? resultaatMappings = null,
        Dictionary<string, Uri>? statusMappings = null,
        Dictionary<string, Uri>? besluittypeMappings = null,
        Dictionary<string, Uri>? documenttypeMappings = null)
    {
        var zaaktypeUrl = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/zaaktypen/{ZaaktypeId}");
        var pdfInfoObjectTypeUrl = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{PdfInfoObjectTypeId}");

        return new MigrateZaakMappingModel
        {
            ResultaatMapper = new ResultaatMapper(resultaatMappings ?? new Dictionary<string, Uri>()),
            StatusMapper = new StatusMapper(statusMappings ?? new Dictionary<string, Uri>()),
            ZaakMapper = new ZaakMapper(Rsin, zaaktypeUrl, new Dictionary<bool, ZaakVertrouwelijkheidaanduiding>
            {
                { false, ZaakVertrouwelijkheidaanduiding.openbaar },
                { true, ZaakVertrouwelijkheidaanduiding.vertrouwelijk }
            }),
            DocumentMapper = new DocumentMapper(
                Rsin,
                new Dictionary<string, DocumentStatus> { { "Definitief", DocumentStatus.definitief } },
                new Dictionary<string, DocumentVertrouwelijkheidaanduiding> { { "Publiek", DocumentVertrouwelijkheidaanduiding.openbaar } },
                documenttypeMappings ?? new Dictionary<string, Uri>()),
            BesluitMapper = new BesluitMapper(Rsin, besluittypeMappings ?? new Dictionary<string, Uri>()),
            PdfMapper = new PdfMapper(Rsin, pdfInfoObjectTypeUrl),
            RolMapper = new([])
        };
    }

    #endregion
}

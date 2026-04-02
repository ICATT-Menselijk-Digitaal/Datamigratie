using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Pdf;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak;

public class MigrateRollenTests
{
    private const string BehandelaarRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/behandelaar-uuid";
    private const string ZaakUrl = "https://openzaak.example.com/zaken/api/v1/zaken/zaak-uuid";
    private const string ZaaktypeUrl = "https://openzaak.example.com/catalogi/api/v1/zaaktypen/zaaktype-uuid";
    private const string IotypeUrl = "https://openzaak.example.com/catalogi/api/v1/informatieobjecttypen/iotype-uuid";

    private static MigrateZaakService CreateService(Mock<IOpenZaakApiClient> openZaakClientMock)
    {
        var detClientMock = new Mock<IDetApiClient>();
        var pdfGeneratorMock = new Mock<IZaakgegevensPdfGenerator>();
        pdfGeneratorMock.Setup(g => g.GenerateZaakgegevensPdf(It.IsAny<DetZaak>()))
            .Returns([]);

        var options = Options.Create(new OpenZaakApiOptions
        {
            BaseUrl = "https://openzaak.example.com/",
            ApiKey = "test-key",
            ApiUser = "test-user"
        });

        return new MigrateZaakService(
            openZaakClientMock.Object,
            detClientMock.Object,
            options,
            pdfGeneratorMock.Object,
            NullLogger<MigrateZaakService>.Instance);
    }

    private static Mock<IOpenZaakApiClient> CreateOpenZaakClientMock()
    {
        var mock = new Mock<IOpenZaakApiClient>();

        mock.Setup(c => c.GetZaakByIdentificatie(It.IsAny<string>()))
            .ReturnsAsync((OzZaak?)null);

        mock.Setup(c => c.CreateZaak(It.IsAny<CreateOzZaakRequest>()))
            .ReturnsAsync(new OzZaak
            {
                Url = new Uri(ZaakUrl),
                Zaaktype = new Uri(ZaaktypeUrl)
            });

        mock.Setup(c => c.GetInformatieobjecttypenUrlsForZaaktype(It.IsAny<Uri>()))
            .ReturnsAsync([new Uri(IotypeUrl)]);

        mock.Setup(c => c.CreateDocument(It.IsAny<OzDocument>()))
            .ReturnsAsync(new OzDocument
            {
                Url = "https://openzaak.example.com/documenten/api/v1/enkelvoudiginformatieobjecten/00000000-0000-0000-0000-000000000001",
                Bestandsnaam = "test.pdf",
                Bestandsomvang = 0,
                Inhoud = null,
                Bestandsdelen = [],
                Bronorganisatie = "123456789",
                Creatiedatum = DateOnly.FromDateTime(DateTime.Today),
                Titel = "Test",
                Vertrouwelijkheidaanduiding = DocumentVertrouwelijkheidaanduiding.openbaar,
                Auteur = "test",
                Status = DocumentStatus.definitief,
                Taal = "nld",
                Link = "",
                Beschrijving = "",
                Verschijningsvorm = "",
                Informatieobjecttype = new Uri(IotypeUrl),
                Trefwoorden = []
            });

        mock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    private static MigrateZaakMappingModel CreateMapping(Dictionary<DetRolType, Uri>? roltypeMappings = null) =>
        new()
        {
            Rsin = "123456789",
            DocumentstatusMappings = new Dictionary<string, string>(),
            ZaakVertrouwelijkheidMappings = new Dictionary<bool, ZaakVertrouwelijkheidaanduiding>
            {
                { false, ZaakVertrouwelijkheidaanduiding.openbaar },
                { true, ZaakVertrouwelijkheidaanduiding.vertrouwelijk }
            },
            BesluittypeMappings = new Dictionary<string, Guid>(),
            PdfInformatieobjecttypeId = Guid.NewGuid(),
            RoltypeMappings = roltypeMappings ?? new ()
            {
                { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) }
            }
        };

    private static DetZaak CreateDetZaak(string? behandelaar = "medewerker-123") =>
        new()
        {
            FunctioneleIdentificatie = "ZAAK-2024-0000001",
            Omschrijving = "Test zaak",
            Open = false,
            Behandelaar = behandelaar,
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            CreatieDatumTijd = DateTimeOffset.UtcNow,
            Historie = []
        };

    [Fact]
    public async Task MigrateZaak_WithBehandelaarAndRoltype_CreatesRolInOpenZaak()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        await service.MigrateZaak(CreateDetZaak("medewerker-123"), CreateMapping());

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.medewerker &&
                r.Roltype == new Uri(BehandelaarRoltypeUrl) &&
                r.BetrokkeneIdentificatie.Identificatie == "medewerker-123" &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithoutBehandelaar_DoesNotCreateRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        await service.MigrateZaak(CreateDetZaak(behandelaar: null), CreateMapping());

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WhenRoltypeIsAlleenPdf_DoesNotCreateRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        // Alleen-PDF rollen are excluded from the dictionary by ValidateRoltypeMappingsService
        var mapping = CreateMapping([]);

        await service.MigrateZaak(CreateDetZaak("medewerker-123"), mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WhenNoBehandelaarMapping_DoesNotCreateRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping([]);

        await service.MigrateZaak(CreateDetZaak("medewerker-123"), mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithUnknownRolKey_DoesNotCreateRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.initiator, new Uri("https://openzaak.example.com/catalogi/api/v1/roltypen/initiator-uuid") }
        });

        await service.MigrateZaak(CreateDetZaak("medewerker-123"), mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithBetrokkenePersoon_CreatesNatuurlijkPersoonRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string belanghebbendeRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/belanghebbende-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.belanghebbende, new Uri(belanghebbendeRoltypeUrl) }
        });

        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.belanghebbende,
                Betrokkene = new DetBetrokkenePersoon
                {
                    Subjecttype = DetSubjecttype.persoon,
                    BurgerServiceNummer = "123456789"
                }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.natuurlijk_persoon &&
                r.Roltype == new Uri(belanghebbendeRoltypeUrl) &&
                r.BetrokkeneIdentificatie.InpBsn == "123456789" &&
                r.IndicatieMachtiging == null &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithBetrokkeneBedrijf_CreatesNietNatuurlijkPersoonRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string melderRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/melder-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.melder, new Uri(melderRoltypeUrl) }
        });

        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = new DetBetrokkenePersoon
                {
                    Subjecttype = DetSubjecttype.bedrijf,
                    KvkNummer = "12345678",
                    Vestigingsnummer = "000012345678"
                }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.niet_natuurlijk_persoon &&
                r.Roltype == new Uri(melderRoltypeUrl) &&
                r.BetrokkeneIdentificatie.KvkNummer == "12345678" &&
                r.BetrokkeneIdentificatie.VestigingsNummer == "000012345678" &&
                r.IndicatieMachtiging == null &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithGemachtigde_SetsIndicatieMachtiging()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string gemachtigdeRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/gemachtigde-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.gemachtigde, new Uri(gemachtigdeRoltypeUrl) }
        });

        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.gemachtigde,
                Betrokkene = new DetBetrokkenePersoon
                {
                    Subjecttype = DetSubjecttype.persoon,
                    BurgerServiceNummer = "987654321"
                }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.natuurlijk_persoon &&
                r.Roltype == new Uri(gemachtigdeRoltypeUrl) &&
                r.BetrokkeneIdentificatie.InpBsn == "987654321" &&
                r.IndicatieMachtiging == "gemachtigde" &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithBetrokkeneAlleenPdf_DoesNotCreateRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        // Initiator is alleen-PDF: absent from the dictionary
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.initiator,
                Betrokkene = new DetBetrokkenePersoon { Subjecttype = DetSubjecttype.persoon, BurgerServiceNummer = "111222333" }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithBetrokkeneUnknownSubjecttype_SkipsBetrokkene()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string melderRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/melder-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.melder, new Uri(melderRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = new DetBetrokkenePersoon { Subjecttype = null }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithPersoonMissingBsn_SkipsBetrokkene()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string belanghebbendeRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/belanghebbende-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.belanghebbende, new Uri(belanghebbendeRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.belanghebbende,
                Betrokkene = new DetBetrokkenePersoon { Subjecttype = DetSubjecttype.persoon, BurgerServiceNummer = null }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithBedrijfMissingKvkNummer_SkipsBetrokkene()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        const string melderRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/melder-uuid";
        var mapping = CreateMapping(new Dictionary<DetRolType, Uri>
        {
            { DetRolType.behandelaar, new Uri(BehandelaarRoltypeUrl) },
            { DetRolType.melder, new Uri(melderRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = new DetBetrokkenePersoon { Subjecttype = DetSubjecttype.bedrijf, KvkNummer = null }
            }
        ];

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private const string InitiatorRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/initiator-uuid";

    [Fact]
    public async Task MigrateZaak_WithInitiatorPersoon_CreatesNatuurlijkPersoonRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = DetSubjecttype.persoon, BurgerServiceNummer = "123456789" };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.natuurlijk_persoon &&
                r.Roltype == new Uri(InitiatorRoltypeUrl) &&
                r.BetrokkeneIdentificatie.InpBsn == "123456789" &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithInitiatorBedrijf_CreatesNietNatuurlijkPersoonRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = DetSubjecttype.bedrijf, KvkNummer = "87654321", Vestigingsnummer = "000087654321" };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(
            It.Is<OzCreateRolRequest>(r =>
                r.BetrokkeneType == BetrokkeneType.niet_natuurlijk_persoon &&
                r.Roltype == new Uri(InitiatorRoltypeUrl) &&
                r.BetrokkeneIdentificatie.KvkNummer == "87654321" &&
                r.BetrokkeneIdentificatie.VestigingsNummer == "000087654321" &&
                r.Zaak == new Uri(ZaakUrl)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MigrateZaak_WithInitiatorNull_DoesNotCreateInitiatorRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = null;

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithInitiatorMissingSubjecttype_DoesNotCreateInitiatorRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = null, BurgerServiceNummer = "123456789" };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithInitiatorPersoonMissingBsn_SkipsInitiator()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = DetSubjecttype.persoon, BurgerServiceNummer = null };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithInitiatorBedrijfMissingKvkNummer_SkipsInitiator()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Initiator", new Uri(InitiatorRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = DetSubjecttype.bedrijf, KvkNummer = null };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MigrateZaak_WithNoInitiatorMapping_DoesNotCreateInitiatorRol()
    {
        var clientMock = CreateOpenZaakClientMock();
        var service = CreateService(clientMock);

        // No "Initiator" key in mappings
        var mapping = CreateMapping(new Dictionary<string, Uri>
        {
            { "Behandelaar", new Uri(BehandelaarRoltypeUrl) }
        });

        var zaak = CreateDetZaak(behandelaar: null);
        zaak.Initiator = new DetInitiator { Subjecttype = DetSubjecttype.persoon, BurgerServiceNummer = "123456789" };

        await service.MigrateZaak(zaak, mapping);

        clientMock.Verify(c => c.CreateRol(It.IsAny<OzCreateRolRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

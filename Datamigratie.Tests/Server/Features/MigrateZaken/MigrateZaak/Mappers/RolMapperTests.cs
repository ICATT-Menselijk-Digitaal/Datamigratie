using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class RolMapperTests
{
    private static readonly Uri s_behandelaarRoltypeUrl = new("https://openzaak.example.com/catalogi/api/v1/roltypen/behandelaar-uuid");
    private static readonly Uri s_initiatorRoltypeUrl = new("https://openzaak.example.com/catalogi/api/v1/roltypen/initiator-uuid");
    private static readonly Uri s_belangRoltypeUrl = new("https://openzaak.example.com/catalogi/api/v1/roltypen/belanghebbende-uuid");
    private static readonly Uri s_melderRoltypeUrl = new("https://openzaak.example.com/catalogi/api/v1/roltypen/melder-uuid");
    private static readonly Uri s_gemachtigdeRoltypeUrl = new("https://openzaak.example.com/catalogi/api/v1/roltypen/gemachtigde-uuid");
    private static readonly Uri s_openZaakZaakUri = new("https://openzaak.example.com/zaken/api/v1/zaken/12345678-1234-1234-1234-123456789012");

    private static DetZaak CreateDetZaak(string? behandelaar = null) => new()
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

    // --- Behandelaar ---

    [Fact]
    public void MapRoles_WithBehandelaarAndMapping_ReturnsMedewerkerRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.behandelaar, s_behandelaarRoltypeUrl } });
        var zaak = CreateDetZaak("medewerker-123");

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.medewerker, rol.BetrokkeneType);
        Assert.Equal(s_behandelaarRoltypeUrl, rol.Roltype);
        Assert.Equal("medewerker-123", rol.BetrokkeneIdentificatie.Identificatie);
    }

    [Fact]
    public void MapRoles_WithoutBehandelaar_ReturnsNoRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.behandelaar, s_behandelaarRoltypeUrl } });
        var zaak = CreateDetZaak(behandelaar: null);

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithBehandelaarButNoMapping_ReturnsNoRol()
    {
        var mapper = new RolMapper([]);
        var zaak = CreateDetZaak("medewerker-123");

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    // --- Betrokkenen ---

    [Fact]
    public void MapRoles_WithBetrokkenePersoon_ReturnsNatuurlijkPersoonRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.belanghebbende, s_belangRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.belanghebbende,
                Betrokkene = CreateDetPersoon("123456789")
            }
        ];

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.natuurlijk_persoon, rol.BetrokkeneType);
        Assert.Equal(s_belangRoltypeUrl, rol.Roltype);
        Assert.Equal("123456789", rol.BetrokkeneIdentificatie.InpBsn);
        Assert.Null(rol.IndicatieMachtiging);
    }

    [Fact]
    public void MapRoles_WithBetrokkeneBedrijf_ReturnsNietNatuurlijkPersoonRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.melder, s_melderRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = CreateDetBedrijf("12345678", "000012345678")
            }
        ];

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.niet_natuurlijk_persoon, rol.BetrokkeneType);
        Assert.Equal(s_melderRoltypeUrl, rol.Roltype);
        Assert.Equal("12345678", rol.BetrokkeneIdentificatie.KvkNummer);
        Assert.Equal("000012345678", rol.BetrokkeneIdentificatie.VestigingsNummer);
        Assert.Null(rol.IndicatieMachtiging);
    }

    [Fact]
    public void MapRoles_WithGemachtigde_SetsIndicatieMachtiging()
    {
        var mapper = new RolMapper(new() { { DetRolType.gemachtigde, s_gemachtigdeRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.gemachtigde,
                Betrokkene = CreateDetPersoon("987654321")
            }
        ];

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.natuurlijk_persoon, rol.BetrokkeneType);
        Assert.Equal(s_gemachtigdeRoltypeUrl, rol.Roltype);
        Assert.Equal("987654321", rol.BetrokkeneIdentificatie.InpBsn);
        Assert.Equal("gemachtigde", rol.IndicatieMachtiging);
    }

    [Fact]
    public void MapRoles_WithBetrokkeneNoMapping_ReturnsNoRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.behandelaar, s_behandelaarRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.initiator,
                Betrokkene = CreateDetPersoon("111222333")
            }
        ];

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithBetrokkeneUnknownSubjecttype_ReturnsNoRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.melder, s_melderRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = CreateDetPersoon()
            }
        ];

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithPersoonMissingBsn_ReturnsNoRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.belanghebbende, s_belangRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.belanghebbende,
                Betrokkene = CreateDetPersoon()
            }
        ];

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithBedrijfMissingKvkNummer_ReturnsNoRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.melder, s_melderRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Betrokkenen =
        [
            new DetBetrokkene
            {
                IndCorrespondentie = false,
                TypeBetrokkenheid = DetRolType.melder,
                Betrokkene = CreateDetBedrijf()
            }
        ];

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    // --- Initiator ---

    [Fact]
    public void MapRoles_WithInitiatorPersoon_ReturnsNatuurlijkPersoonRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.initiator, s_initiatorRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = CreateDetPersoon("123456789");

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.natuurlijk_persoon, rol.BetrokkeneType);
        Assert.Equal(s_initiatorRoltypeUrl, rol.Roltype);
        Assert.Equal("123456789", rol.BetrokkeneIdentificatie.InpBsn);
    }

    [Fact]
    public void MapRoles_WithInitiatorBedrijf_ReturnsNietNatuurlijkPersoonRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.initiator, s_initiatorRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = CreateDetBedrijf("87654321", "000087654321");

        var rollen = mapper.MapRoles(zaak, s_openZaakZaakUri).ToList();

        var rol = Assert.Single(rollen);
        Assert.Equal(BetrokkeneType.niet_natuurlijk_persoon, rol.BetrokkeneType);
        Assert.Equal(s_initiatorRoltypeUrl, rol.Roltype);
        Assert.Equal("87654321", rol.BetrokkeneIdentificatie.KvkNummer);
        Assert.Equal("000087654321", rol.BetrokkeneIdentificatie.VestigingsNummer);
    }

    [Fact]
    public void MapRoles_WithInitiatorNull_ReturnsNoInitiatorRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.initiator, s_initiatorRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = null;

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithInitiatorPersoonMissingBsn_ReturnsNoInitiatorRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.initiator, s_initiatorRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = CreateDetPersoon();

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithInitiatorBedrijfMissingKvkNummer_ReturnsNoInitiatorRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.initiator, s_initiatorRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = CreateDetBedrijf();

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    [Fact]
    public void MapRoles_WithNoInitiatorMapping_ReturnsNoInitiatorRol()
    {
        var mapper = new RolMapper(new() { { DetRolType.behandelaar, s_behandelaarRoltypeUrl } });
        var zaak = CreateDetZaak();
        zaak.Initiator = CreateDetPersoon("123456789");

        Assert.Empty(mapper.MapRoles(zaak, s_openZaakZaakUri));
    }

    private static DetPersoon CreateDetPersoon(string? bsn = null) =>
        new()
        {
            HandmatigToegevoegd = false,
            Geblokkeerd = false,
            CurateleRegister = false,
            InOnderzoek = false,
            BeperkingVerstrekking = false,
            AfnemerIndicatie = false,
            BurgerServiceNummer = bsn
        };

    private static DetBedrijf CreateDetBedrijf(string? kvkNummer = null, string? vestigingsnummer = null) =>
        new()
        {
            HandmatigToegevoegd = false,
            InSurceance = false,
            Failliet = false,
            Ingangsdatum = new DateOnly(2024, 1, 1),
            Vestigingstype = "hoofd",
            KvkNummer = kvkNummer,
            Vestigingsnummer = vestigingsnummer
        };
}

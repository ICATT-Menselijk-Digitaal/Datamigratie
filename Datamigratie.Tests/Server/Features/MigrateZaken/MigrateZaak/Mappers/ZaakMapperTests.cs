using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class ZaakMapperTests
{
    private const string OpenZaakBaseUrl = "https://openzaak.example.com/";
    private const string Rsin = "123456782";
    private static readonly Guid ZaaktypeId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Uri ZaaktypeUrl = new($"{OpenZaakBaseUrl}catalogi/api/v1/zaaktypen/{ZaaktypeId}");

    private static ZaakMapper CreateMapper(Dictionary<bool, ZaakVertrouwelijkheidaanduiding>? mappings = null)
    {
        return new ZaakMapper(Rsin, ZaaktypeUrl, mappings ?? new Dictionary<bool, ZaakVertrouwelijkheidaanduiding>
        {
            { false, ZaakVertrouwelijkheidaanduiding.openbaar },
            { true, ZaakVertrouwelijkheidaanduiding.vertrouwelijk }
        });
    }

    [Fact]
    public void Map_ValidInput_MapsAllFields()
    {
        var mapper = CreateMapper();

        var detZaak = CreateMinimalDetZaak(
            identificatie: "ZAAK-001",
            omschrijving: "Test zaak",
            externeIdentificatie: "EXT-001");

        var result = mapper.Map(detZaak);

        Assert.Equal("ZAAK-001", result.Identificatie);
        Assert.Equal(Rsin, result.Bronorganisatie);
        Assert.Equal(Rsin, result.VerantwoordelijkeOrganisatie);
        Assert.Equal("Test zaak", result.Omschrijving);
        Assert.Equal($"{OpenZaakBaseUrl}catalogi/api/v1/zaaktypen/{ZaaktypeId}", result.Zaaktype.ToString());
        Assert.Equal(ZaakVertrouwelijkheidaanduiding.openbaar, result.Vertrouwelijkheidaanduiding);
        Assert.NotNull(result.Kenmerken);
        Assert.Single(result.Kenmerken);
        Assert.Equal("EXT-001", result.Kenmerken[0].Kenmerk);
        Assert.Equal("e-Suite", result.Kenmerken[0].Bron);
    }

    [Fact]
    public void Map_OmschrijvingTruncated()
    {
        var mapper = CreateMapper();
        var longOmschrijving = new string('A', 100);

        var detZaak = CreateMinimalDetZaak(omschrijving: longOmschrijving);

        var result = mapper.Map(detZaak);

        Assert.Equal(80, result.Omschrijving!.Length);
        Assert.EndsWith("...", result.Omschrijving);
    }

    [Fact]
    public void Map_ZaaknummerTooLong_Throws()
    {
        var mapper = CreateMapper();
        var longId = new string('Z', 41);

        var detZaak = CreateMinimalDetZaak(identificatie: longId);

        var ex = Assert.Throws<InvalidDataException>(() =>
            mapper.Map(detZaak));
        Assert.Contains("zaaknummer", ex.Message);
    }

    [Fact]
    public void Map_NoExterneIdentificatie_KenmerkenNull()
    {
        var mapper = CreateMapper();

        var detZaak = CreateMinimalDetZaak(externeIdentificatie: null);

        var result = mapper.Map(detZaak);

        Assert.Null(result.Kenmerken);
    }

    [Fact]
    public void Map_OnlyBewaartermijnEinddatum_MapsArchiefactiedatum()
    {
        var mapper = CreateMapper();
        var detZaak = CreateMinimalDetZaak();
        detZaak.ArchiveerGegevens = new DetArchiveerGegevens
        {
            BewaartermijnEinddatum = new DateOnly(2030, 6, 15)
        };

        var result = mapper.Map(detZaak);

        Assert.Equal("2030-06-15", result.Archiefactiedatum);
    }

    [Fact]
    public void Map_OnlyOverbrengenOp_MapsArchiefactiedatum()
    {
        var mapper = CreateMapper();
        var detZaak = CreateMinimalDetZaak();
        detZaak.ArchiveerGegevens = new DetArchiveerGegevens
        {
            OverbrengenOp = new DateOnly(2031, 3, 20)
        };

        var result = mapper.Map(detZaak);

        Assert.Equal("2031-03-20", result.Archiefactiedatum);
    }

    [Fact]
    public void Map_NoArchiveerGegevens_ArchiefactiedatumNull()
    {
        var mapper = CreateMapper();
        var detZaak = CreateMinimalDetZaak();

        var result = mapper.Map(detZaak);

        Assert.Null(result.Archiefactiedatum);
    }

    [Fact]
    public void Map_BothBewaartermijnAndOverbrengenOp_Throws()
    {
        var mapper = CreateMapper();
        var detZaak = CreateMinimalDetZaak();
        detZaak.ArchiveerGegevens = new DetArchiveerGegevens
        {
            BewaartermijnEinddatum = new DateOnly(2030, 1, 1),
            OverbrengenOp = new DateOnly(2031, 1, 1)
        };

        var ex = Assert.Throws<InvalidDataException>(() => mapper.Map(detZaak));
        Assert.Contains("bewaartermijnEinddatum", ex.Message);
        Assert.Contains("overbrengenOp", ex.Message);
    }

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
}

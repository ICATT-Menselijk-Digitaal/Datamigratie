using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class StatusMapperTests
{
    [Fact]
    public void Map_ValidMapping_ReturnsPlanWithDate()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/statustypen/bbbb2222-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var mapper = new StatusMapper(new Dictionary<string, Uri> { { "Afgehandeld", uri } });
        var einddatum = new DateOnly(2024, 6, 15);
        var detZaak = new DetZaak
        {
            FunctioneleIdentificatie = "ZAAK-001",
            Omschrijving = "Test",
            CreatieDatumTijd = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            Einddatum = einddatum,
            Historie = []
        };

        var result = mapper.Map(new DetStatus { Uitwisselingscode = "", Naam = "Afgehandeld", Actief = false }, detZaak);

        Assert.NotNull(result);
        Assert.Equal(uri, result.Statustype);
        Assert.Equal(einddatum.ToDateTime(TimeOnly.MinValue), result.DatumStatusGezet);
    }

    [Fact]
    public void Map_UnmappedNaam_ReturnsNull()
    {
        var mapper = new StatusMapper(new Dictionary<string, Uri>());
        var detZaak = new DetZaak
        {
            FunctioneleIdentificatie = "ZAAK-001",
            Omschrijving = "Test",
            CreatieDatumTijd = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            Historie = []
        };

        Assert.Null(mapper.Map(new DetStatus { Uitwisselingscode = "", Naam = "Onbekend", Actief = false }, detZaak));
    }

    [Fact]
    public void Map_NoEinddatum_Throws()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/statustypen/bbbb2222-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var mapper = new StatusMapper(new Dictionary<string, Uri> { { "Afgehandeld", uri } });
        var detZaak = new DetZaak
        {
            FunctioneleIdentificatie = "ZAAK-001",
            Omschrijving = "Test",
            CreatieDatumTijd = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Startdatum = new DateOnly(2024, 1, 1),
            Streefdatum = new DateOnly(2024, 12, 31),
            Einddatum = null,
            Historie = []
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            mapper.Map(new DetStatus { Uitwisselingscode = "", Naam = "Afgehandeld", Actief = false }, detZaak));
        Assert.Contains("einddatum", ex.Message);
    }
}

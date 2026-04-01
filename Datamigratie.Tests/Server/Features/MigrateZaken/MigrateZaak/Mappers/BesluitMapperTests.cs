using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class BesluitMapperTests
{
    private const string OpenZaakBaseUrl = "https://openzaak.example.com/";
    private const string Rsin = "123456782";

    [Fact]
    public void Map_ValidBesluit_MapsFields()
    {
        var besluitGuid = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var besluitUri = new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/besluittypen/{besluitGuid}");
        var mapper = new BesluitMapper(Rsin, new Dictionary<string, Uri> { { "Toekenning", besluitUri } });

        var detBesluit = new DetBesluit
        {
            FunctioneleIdentificatie = "BESLUIT-001",
            Besluittype = new DetBesluittype { Naam = "Toekenning" },
            BesluitDatum = new DateOnly(2024, 1, 15),
            Toelichting = "Test toelichting",
            Ingangsdatum = new DateOnly(2024, 2, 1),
            Vervaldatum = new DateOnly(2025, 2, 1),
            Publicatiedatum = new DateOnly(2024, 1, 20),
            Reactiedatum = new DateOnly(2024, 3, 1),
        };

        var result = mapper.Map(detBesluit);

        Assert.Equal("BESLUIT-001", result.Identificatie);
        Assert.Equal(besluitUri, result.Besluittype);
        Assert.Equal(Rsin, result.VerantwoordelijkeOrganisatie);
        Assert.Equal(new DateOnly(2024, 1, 15), result.Datum);
        Assert.Equal("Test toelichting", result.Toelichting);
        Assert.Equal(new DateOnly(2024, 2, 1), result.Ingangsdatum);
        Assert.Equal(new DateOnly(2025, 2, 1), result.Vervaldatum);
        Assert.Equal(new DateOnly(2024, 1, 20), result.Publicatiedatum);
        Assert.Equal(new DateOnly(2024, 3, 1), result.UiterlijkeReactiedatum);
    }

    [Fact]
    public void Map_NoIngangsdatum_FallsBackToMinDate()
    {
        var besluitGuid = Guid.NewGuid();
        var mapper = new BesluitMapper(Rsin, new Dictionary<string, Uri>
        {
            { "Toekenning", new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/besluittypen/{besluitGuid}") }
        });

        var detBesluit = new DetBesluit
        {
            FunctioneleIdentificatie = "BESLUIT-001",
            Besluittype = new DetBesluittype { Naam = "Toekenning" },
            BesluitDatum = new DateOnly(2024, 1, 15),
            Ingangsdatum = null,
        };

        var result = mapper.Map(detBesluit);

        Assert.Equal(new DateOnly(1, 1, 1), result.Ingangsdatum);
    }

    [Fact]
    public void Map_LongIdentificatie_Truncated()
    {
        var besluitGuid = Guid.NewGuid();
        var longId = new string('B', 60);
        var mapper = new BesluitMapper(Rsin, new Dictionary<string, Uri>
        {
            { "Toekenning", new Uri($"{OpenZaakBaseUrl}catalogi/api/v1/besluittypen/{besluitGuid}") }
        });

        var detBesluit = new DetBesluit
        {
            FunctioneleIdentificatie = longId,
            Besluittype = new DetBesluittype { Naam = "Toekenning" },
            BesluitDatum = new DateOnly(2024, 1, 15),
        };

        var result = mapper.Map(detBesluit);

        Assert.Equal(50, result.Identificatie.Length);
        Assert.EndsWith("...", result.Identificatie);
    }
}

using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak.Mappers;

public class ResultaatMapperTests
{
    [Fact]
    public void Map_ValidMapping_ReturnsPlan()
    {
        var uri = new Uri("https://openzaak.example.com/catalogi/api/v1/resultaattypen/aaaa1111-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var mapper = new ResultaatMapper(new Dictionary<string, Uri> { { "Toegekend", uri } });

        var result = mapper.Map(new DetResultaat { Actief = true, Naam = "Toegekend" });

        Assert.NotNull(result);
        Assert.Equal(uri, result.Resultaattype);
    }

    [Fact]
    public void Map_UnmappedNaam_ReturnsNull()
    {
        var mapper = new ResultaatMapper(new Dictionary<string, Uri>());

        Assert.Null(mapper.Map(new DetResultaat { Actief = true, Naam = "Onbekend" }));
    }
}

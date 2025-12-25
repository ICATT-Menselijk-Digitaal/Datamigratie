using System.Reflection;
using System.Text.Json;
using Datamigratie.FakeDet.Catalogi.Omgevingswet;

namespace Datamigratie.FakeDet.Catalogi;

public class GetCatalogusData
{
     public static async Task<ZaaktypecatalogusRoot?> GetCatalogus(string folder)
    {
        await using var stream = GetEmbeddedResource(folder);
        return await JsonSerializer.DeserializeAsync<ZaaktypecatalogusRoot>(stream, serializerOptions);
    }

    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly Assembly s_assembly = typeof(GetCatalogusData).Assembly;
    private static Stream GetEmbeddedResource(string folder) => s_assembly.GetManifestResourceStream($"Datamigratie.FakeDet.Catalogi.{folder}.data.json");
}

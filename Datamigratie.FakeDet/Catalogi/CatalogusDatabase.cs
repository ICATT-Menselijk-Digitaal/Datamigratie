using System.Text.Json;
using Datamigratie.FakeDet.Catalogi.Models;

namespace Datamigratie.FakeDet.Catalogi;

public class CatalogusDatabase
{
    public static async IAsyncEnumerable<ZaaktypeCatalogus> GetCatalogi()
    {
        var folder = Path.Combine(Environment.CurrentDirectory, "Catalogi");
        var allJson = Directory.GetFiles(folder, "*.json");
        foreach (var fn in allJson)
        {
            await using var stream = File.OpenRead(fn);
            var catalogus = await JsonSerializer.DeserializeAsync<ZaaktypeCatalogus>(stream, JsonSerializerOptions.Web);
            if (catalogus != null)
            {
                yield return catalogus;
            }
        }
        
    }
}

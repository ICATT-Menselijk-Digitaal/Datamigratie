using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.FakeDet;
public class ZaakDatabase
{
    public static async IAsyncEnumerable<DetZaakMinimal> GetZakenByZaaktype(string zaaktype, int page)
    {
        const int pageSize = 20;
        var skip = (page - 1) * pageSize;
        
        var encoded = Uri.EscapeDataString(zaaktype);
        var path = Path.Combine(Environment.CurrentDirectory, "Zaken", encoded + ".zip");
        if (!File.Exists(path)) yield break;
        await using var stream = File.OpenRead(path);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
        
        foreach (var entry in zip.Entries.Skip(skip).Take(pageSize))
        {
            await using var openedEntry = await entry.OpenAsync();
            var json = await JsonSerializer.DeserializeAsync<DetZaakMinimal>(openedEntry, JsonSerializerOptions.Web);
            if(json is {}) yield return json;
        }
    }

    public static async Task<DetZaak?> GetZaak(string functioneleIdentificatie)
    {
        var encoded = Uri.EscapeDataString(functioneleIdentificatie);
        var files = GetAllZipFiles();
        var match = files.FirstOrDefault(x =>
        {
            var span = x.AsSpan();
            var lastIndexOf = span.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var type = span[lastIndexOf..^4];
            return encoded.StartsWith(type);
        });
        if(string.IsNullOrWhiteSpace(match)) return null;

        await using var stream = File.OpenRead(match);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
        var entry = zip.GetEntry($"{encoded}.json");
        if(entry is null) return null;
        
        await using var openedEntry = await entry.OpenAsync();
        return await JsonSerializer.DeserializeAsync<DetZaak>(openedEntry, JsonSerializerOptions.Web);
    }

    public static async Task<IReadOnlyDictionary<long, string>> GetDocumentDictionary()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Zaken", "document-inhoud-registry.zip");
        await using var stream = File.OpenRead(path);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
        var entry = zip.Entries[0];
        await using var openedEntry =  await entry.OpenAsync();
        var result = await  JsonSerializer.DeserializeAsync<Dictionary<long, string>>(openedEntry, JsonSerializerOptions.Web);
        return result ?? new();
    }

    private static IEnumerable<string> GetAllZipFiles()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Zaken");
        var files = Directory.GetFiles(path, "*.zip");
        return files.Where(x => !x.EndsWith("document-inhoud-registry.zip", StringComparison.OrdinalIgnoreCase));
    }
}

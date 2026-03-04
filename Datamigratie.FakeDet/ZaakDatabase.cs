using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.FakeDet;

public class ZaakDatabase(IConfiguration configuration)
{
    public const int PageSize = 100;

    private readonly string _outputPath = configuration["ZAKEN_PATH"] is { } p && !string.IsNullOrWhiteSpace(p) ? p : Path.GetFullPath("output");

    public async Task<int> GetZakenCountByZaaktype(string zaaktype)
    {
        var path = GetZaaktypeZipPath(zaaktype);
        if (!File.Exists(path)) return 0;
        await using var stream = File.OpenRead(path);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
        return zip.Entries.Count;
    }

    public async IAsyncEnumerable<DetZaakMinimal> GetZakenByZaaktype(string zaaktype, int page)
    {
        var skip = (page - 1) * PageSize;

        var path = GetZaaktypeZipPath(zaaktype);
        if (!File.Exists(path)) yield break;
        await using var stream = File.OpenRead(path);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);

        foreach (var entry in zip.Entries.Skip(skip).Take(PageSize))
        {
            await using var openedEntry = await entry.OpenAsync();
            var json = await JsonSerializer.DeserializeAsync<DetZaakMinimal>(openedEntry, JsonSerializerOptions.Web);
            if (json is { }) yield return json;
        }
    }

    private string GetZaaktypeZipPath(string zaaktype)
    {
        var encoded = Uri.EscapeDataString(zaaktype);
        var path = Path.Combine(_outputPath, encoded + ".zip");
        return path;
    }
    public async Task<DetZaak?> GetZaak(string functioneleIdentificatie)
    {
        var entryName = $"{functioneleIdentificatie}.json";
        foreach (var file in GetAllZipFiles())
        {
            await using var stream = File.OpenRead(file);
            await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
            var entry = zip.GetEntry(entryName);
            if (entry is null) continue;

            await using var openedEntry = await entry.OpenAsync();
            return await JsonSerializer.DeserializeAsync<DetZaak>(openedEntry, JsonSerializerOptions.Web);
        }
        return null;
    }

    public async Task<IReadOnlyDictionary<long, string>> GetDocumentDictionary()
    {
        var path = Path.Combine(_outputPath, "document-inhoud-registry.zip");
        await using var stream = File.OpenRead(path);
        await using var zip = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, Encoding.UTF8);
        var entry = zip.Entries[0];
        await using var openedEntry = await entry.OpenAsync();
        var result = await JsonSerializer.DeserializeAsync<Dictionary<long, string>>(openedEntry, JsonSerializerOptions.Web);
        return result ?? new();
    }

    private IEnumerable<string> GetAllZipFiles()
    {
        var files = Directory.GetFiles(_outputPath, "*.zip");
        return files.Where(x => !x.EndsWith("document-inhoud-registry.zip", StringComparison.OrdinalIgnoreCase));
    }
}

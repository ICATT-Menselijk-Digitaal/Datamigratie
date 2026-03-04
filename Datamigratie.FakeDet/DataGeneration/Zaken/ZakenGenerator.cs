using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Datamigratie.FakeDet.Catalogi;
using Datamigratie.FakeDet.DataGeneration.Zaken.Fakers;
using Datamigratie.FakeDet.DataGeneration.Zaken.Models;

namespace Datamigratie.FakeDet.DataGeneration.Zaken;

public class ZakenGenerator(IConfiguration configuration, ILogger<ZakenGenerator> logger)
{
    private readonly int _zakenPerType = int.TryParse(configuration["ZAKEN_PER_TYPE"], out var count) ? count : 5;
    private readonly int _seed = int.TryParse(configuration["ZAKEN_SEED"], out var s) ? s : 12345;
    private readonly string _outputPath = configuration["ZAKEN_PATH"] is  {} p && !string.IsNullOrWhiteSpace(p) ? p : Path.GetFullPath("output");

    public async Task Generate(int? zakenPerType = null)
    {
        zakenPerType ??= _zakenPerType;
        if(Directory.Exists(_outputPath) && Directory.GetFiles(_outputPath).Length > 0)
        {
            logger.LogInformation("zaken are already generated in {outputPath}. First delete them to generate more", _outputPath);
            return;
        }

        logger.LogInformation($"ZakenGenerator - Generating mock zaken from zaaktype catalogi");
        logger.LogInformation("Zaken per type: {zakenPerType}", zakenPerType);
        logger.LogInformation("Random seed: {seed}", _seed);

        var faker = new ZaakFaker(_seed);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Phase 1: Generate all zaken (without gekoppeldeZaken)
        logger.LogInformation("Phase 1: Generating zaken...");
        var zakenByZaaktype = new Dictionary<string, List<Zaak>>();
        var zaaktypeByIdentificatie = new Dictionary<string, Catalogi.Models.Zaaktype>();

        await foreach (var catalogus in CatalogusDatabase.GetCatalogi())
        {
            foreach (var zaaktype in catalogus.Zaaktypen)
            {
                if (string.IsNullOrEmpty(zaaktype.Identificatie))
                    continue;

                zaaktypeByIdentificatie[zaaktype.Identificatie] = zaaktype;
                var generatedZaken = new List<Zaak>();

                logger.LogInformation("Generating {zakenPerType} zaken for: {zaaktype.Omschrijving} ({zaaktype.Domein})", zakenPerType, zaaktype.Omschrijving, zaaktype.Domein);

                for (var i = 0; i < zakenPerType; i++)
                {
                    var zaak = faker.GenerateZaak(zaaktype);
                    generatedZaken.Add(zaak);
                }

                zakenByZaaktype[zaaktype.Identificatie] = generatedZaken;
            }
        }

        // Phase 2: Populate gekoppeldeZaken based on allowed types and existing zaken
        logger.LogInformation("Phase 2: Linking zaken based on gerelateerdeZaaktypen...");
        var zakenByZaaktypeReadOnly = zakenByZaaktype.ToDictionary(
            kvp => kvp.Key,
            kvp => (IReadOnlyList<Zaak>)kvp.Value);

        var linkedCount = 0;
        foreach (var (zaaktypeId, zaken) in zakenByZaaktype)
        {
            var zaaktype = zaaktypeByIdentificatie[zaaktypeId];

            for (var i = 0; i < zaken.Count; i++)
            {
                var updatedZaak = faker.PopulateGekoppeldeZaken(zaken[i], zaaktype, zakenByZaaktypeReadOnly);
                if (updatedZaak.GekoppeldeZaken is not null)
                {
                    zaken[i] = updatedZaak;
                    linkedCount++;
                }
            }
        }
        logger.LogInformation("Linked {linkedCount} zaken to related zaken", linkedCount);

        // Phase 3: Write output files (one JSON per zaak, zipped per zaaktype)
        logger.LogInformation("Phase 3: Writing output files...");

        Directory.CreateDirectory(_outputPath);

        foreach (var (zaaktypeId, zaken) in zakenByZaaktype)
        {
            var zipPath = Path.Combine(_outputPath, $"{Uri.EscapeDataString(zaaktypeId)}.zip");

            await using var zipStream = File.Create(zipPath);
            await using var archive = await ZipArchive.CreateAsync(zipStream, ZipArchiveMode.Create, false, Encoding.UTF8);

            foreach (var zaak in zaken)
            {
                var entryName = $"{zaak.FunctioneleIdentificatie}.json";
                var entry = archive.CreateEntry(entryName, CompressionLevel.SmallestSize);

                await using var entryStream = await entry.OpenAsync();
                await JsonSerializer.SerializeAsync(entryStream, zaak, jsonOptions);
            }
        }
        // Write documentInhoudID registry as separate JSON file
        var registryPath = Path.Combine(_outputPath, "document-inhoud-registry.zip");
        await using var registryStream = File.Create(registryPath);
        await using var zip = await ZipArchive.CreateAsync(registryStream, ZipArchiveMode.Create, false, Encoding.UTF8);
        var registryEntry = zip.CreateEntry("document-inhoud-registry.json", CompressionLevel.SmallestSize);
        await using var registryEntryStream = await registryEntry.OpenAsync();
        await JsonSerializer.SerializeAsync(registryEntryStream, faker.DocumentInhoudRegistry, jsonOptions);
        logger.LogInformation("Written {faker.DocumentInhoudRegistry.Count} document-to-zaak mappings to {registryPath}", faker.DocumentInhoudRegistry.Count, registryPath);

        logger.LogInformation("Done! Generated {zakenCount} zaken across {zakenByZaaktypeCount} zaaktypen.", zakenByZaaktype.Values.Sum(z => z.Count), zakenByZaaktype.Count);

    }

    public void Delete()
    {
        logger.LogInformation("deleting output path {outputPath}", _outputPath);
        if (!Directory.Exists(_outputPath)) return;
        foreach (var file in Directory.GetFiles(_outputPath))
        {
            File.Delete(file);
        }
    }
}

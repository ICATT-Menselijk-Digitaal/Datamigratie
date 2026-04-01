using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documenttype;

public interface IValidateDocumenttypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetDocumenttypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateDocumenttypeMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateDocumenttypeMappingsService> logger,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IValidateDocumenttypeMappingsService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;

    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetDocumenttypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var documenttypen = detZaaktype.Documenttypen?.ToList() ?? [];

        if (documenttypen.Count == 0)
        {
            return (true, new Dictionary<string, Uri>());
        }

        var rawMappings = await dbContext.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "documenttype" && m.SourceId != null)
            .ToDictionaryAsync(x => x.SourceId!, m => m.TargetId);

        var missingDocumenttypen = documenttypen
            .Where(dt => !rawMappings.Any(m => m.Key == dt.Documenttype.Naam))
            .Select(dt => dt.Documenttype.Naam)
            .ToList();

        if (missingDocumenttypen.Count != 0)
        {
            logger.LogWarning("Missing documenttype mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}. Missing {Count} documenttypes: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, missingDocumenttypen.Count, string.Join(", ", missingDocumenttypen.Take(10)));
            return (false, new Dictionary<string, Uri>());
        }

        var parsedMappings = new Dictionary<string, Uri>();
        foreach (var (key, value) in rawMappings)
        {
            if (Guid.TryParse(value, out var guid))
            {
                parsedMappings[key] = new Uri($"{_openZaakBaseUrl}catalogi/api/v1/informatieobjecttypen/{guid}");
            }
            else
            {
                throw new InvalidOperationException(
                    $"Mapped informatieobjecttype value '{value}' for documenttype '{key}' is not a valid GUID.");
            }
        }

        return (true, parsedMappings);
    }
}

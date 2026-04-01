using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;

public interface IValidateResultaattypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetResultaattypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateResultaattypeMappingsService(
    DatamigratieDbContext context,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IValidateResultaattypeMappingsService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;

    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetResultaattypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var detResultaattypen = detZaaktype.Resultaten?
            .Select(r => r.Resultaat.Naam)
            .ToList() ?? [];

        // If no resultaattypen for det zaaktype, consider it valid
        if (detResultaattypen.Count == 0)
        {
            return (true, new Dictionary<string, Uri>());
        }

        var rawMappings = await context.PropertyMappings
            .Where(rm => rm.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && rm.Property == "resultaattype")
            .ToDictionaryAsync(x => x.SourceId, x => x.TargetId);

        // checking if all DET resultaattypen are mapped
        var allMapped = detResultaattypen.All(rawMappings.ContainsKey);

        var mappingDictionary = rawMappings.ToDictionary(
            m => m.Key,
            m => new Uri($"{_openZaakBaseUrl}catalogi/api/v1/resultaattypen/{Guid.Parse(m.Value)}"));

        return (allMapped, mappingDictionary);
    }
}

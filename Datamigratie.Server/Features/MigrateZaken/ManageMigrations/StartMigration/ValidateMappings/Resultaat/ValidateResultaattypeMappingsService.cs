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

        var mappingDictionary = await context.PropertyMappings
            .Where(rm => rm.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && rm.Property == "resultaattype" && rm.SourceId != null)
            .ToDictionaryAsync(x => x.SourceId!, x => new Uri($"{_openZaakBaseUrl}catalogi/api/v1/resultaattypen/{x.TargetId}"));

        // checking if all DET resultaattypen are mapped
        var allMapped = detResultaattypen.All(mappingDictionary.ContainsKey);

        return (allMapped, mappingDictionary);
    }
}

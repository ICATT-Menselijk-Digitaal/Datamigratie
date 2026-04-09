using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;

public interface IValidateStatusMappingsService
{
    Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateStatusMappingsService(
    DatamigratieDbContext context,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IValidateStatusMappingsService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;

    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype)
    {
        var detStatuses = detZaaktype.Statussen
            .Select(s => s.Naam)
            .ToList();

        // If no statuses, consider it valid
        if (detStatuses.Count == 0)
        {
            return (true, new Dictionary<string, Uri>());
        }

        var mappingDictionary = await context.PropertyMappings
            .Where(sm => sm.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && sm.Property == "status" && sm.SourceId != null)
            .ToDictionaryAsync(x => x.SourceId!, x => new Uri($"{_openZaakBaseUrl}catalogi/api/v1/statustypen/{x.TargetId}"));

        // checking if all DET statuses are mapped
        var allMapped = detStatuses.All(status => mappingDictionary.ContainsKey(status));

        return (allMapped, mappingDictionary);
    }
}

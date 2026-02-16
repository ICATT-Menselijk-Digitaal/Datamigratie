using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;

public interface IValidateStatusMappingsService
{
    Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateStatusMappingsService(
    DatamigratieDbContext context) : IValidateStatusMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype)
    {
        var detStatuses = detZaaktype.Statussen
            .Select(s => s.Naam)
            .ToList();

        // If no statuses, consider it valid
        if (detStatuses.Count == 0)
        {
            return (true, new Dictionary<string, Guid>());
        }

        var mappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMapping.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(m => m.DetStatusNaam, m => m.OzStatustypeId);

        // checking if all DET statuses are mapped
        var allMapped = detStatuses.All(status => mappingDictionary.ContainsKey(status));

        return (allMapped, mappingDictionary);
    }
}

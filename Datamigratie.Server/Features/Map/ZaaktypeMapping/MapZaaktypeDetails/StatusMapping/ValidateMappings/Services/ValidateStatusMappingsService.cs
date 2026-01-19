using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.StatusMapping.ValidateMappings.Services;

public interface IValidateStatusMappingsService
{
    Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateStatusMappingsService(
    DatamigratieDbContext context) : IValidateStatusMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Guid> Mappings)> ValidateAndGetStatusMappings(DetZaaktypeDetail detZaaktype)
    {
        var activeDetStatuses = detZaaktype.Statussen
            .Where(s => s.Actief)
            .Select(s => s.Naam)
            .ToList();

        // If no active statuses, consider it valid
        if (activeDetStatuses.Count == 0)
        {
            return (true, new Dictionary<string, Guid>());
        }

        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return (false, new Dictionary<string, Guid>());

        var mappings = await context.StatusMappings
            .Where(sm => sm.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(m => m.DetStatusNaam, m => m.OzStatustypeId);

        // checking if all active DET statuses are mapped
        var allMapped = activeDetStatuses.All(status => mappingDictionary.ContainsKey(status));
        
        return (allMapped, mappingDictionary);
    }
}

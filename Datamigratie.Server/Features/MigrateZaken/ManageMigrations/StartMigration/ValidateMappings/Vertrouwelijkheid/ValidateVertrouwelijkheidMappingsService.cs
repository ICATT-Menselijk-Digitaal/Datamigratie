using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;

public interface IValidateVertrouwelijkheidMappingsService
{
    Task<(bool IsValid, Dictionary<bool, string> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateVertrouwelijkheidMappingsService(
    DatamigratieDbContext context) : IValidateVertrouwelijkheidMappingsService
{
    public async Task<(bool IsValid, Dictionary<bool, string> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype)
    {
        var zaaktypenMapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
            return (false, new Dictionary<bool, string>());

        var mappings = await context.VertrouwelijkheidMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(
            m => m.DetVertrouwelijkheid,
            m => m.OzVertrouwelijkheidaanduiding);

        // Check if both true and false are mapped
        var allMapped = mappingDictionary.ContainsKey(true) && mappingDictionary.ContainsKey(false);

        return (allMapped, mappingDictionary);
    }
}

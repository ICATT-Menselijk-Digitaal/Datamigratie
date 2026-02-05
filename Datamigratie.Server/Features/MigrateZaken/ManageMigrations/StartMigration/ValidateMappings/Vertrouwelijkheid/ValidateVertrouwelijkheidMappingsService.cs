using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;

public interface IValidateVertrouwelijkheidMappingsService
{
    Task<(bool IsValid, Dictionary<bool, VertrouwelijkheidsAanduiding> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateVertrouwelijkheidMappingsService(
    DatamigratieDbContext context) : IValidateVertrouwelijkheidMappingsService
{
    public async Task<(bool IsValid, Dictionary<bool, VertrouwelijkheidsAanduiding> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype)
    {
        var mappings = await context.VertrouwelijkheidMappings
            .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie)
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(
            m => m.DetVertrouwelijkheid,
            m => Enum.Parse<VertrouwelijkheidsAanduiding>(m.OzVertrouwelijkheidaanduiding));

        // Check if both true and false are mapped
        var allMapped = mappingDictionary.ContainsKey(true) && mappingDictionary.ContainsKey(false);

        return (allMapped, mappingDictionary);
    }
}

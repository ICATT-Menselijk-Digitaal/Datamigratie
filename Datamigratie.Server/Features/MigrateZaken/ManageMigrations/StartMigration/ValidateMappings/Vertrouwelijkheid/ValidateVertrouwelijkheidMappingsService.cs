using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;

public interface IValidateVertrouwelijkheidMappingsService
{
    Task<(bool IsValid, Dictionary<bool, ZaakVertrouwelijkheidaanduiding> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateVertrouwelijkheidMappingsService(
    DatamigratieDbContext context) : IValidateVertrouwelijkheidMappingsService
{
    public async Task<(bool IsValid, Dictionary<bool, ZaakVertrouwelijkheidaanduiding> Mappings)> ValidateAndGetVertrouwelijkheidMappings(DetZaaktypeDetail detZaaktype)
    {
        var mappingDictionary = await context.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "vertrouwelijkheid" && m.SourceId != null)
            .ToDictionaryAsync(
                m => bool.Parse(m.SourceId!),
                m => Enum.Parse<ZaakVertrouwelijkheidaanduiding>(m.TargetId));

        // Check if both true and false are mapped
        var allMapped = mappingDictionary.ContainsKey(true) && mappingDictionary.ContainsKey(false);

        return (allMapped, mappingDictionary);
    }
}

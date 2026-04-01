using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;

public interface IValidateBesluittypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetBesluittypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateBesluittypeMappingsService(
    DatamigratieDbContext context,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IValidateBesluittypeMappingsService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;

    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetBesluittypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var detBesluittypen = detZaaktype.Besluiten
            .Select(b => b.Besluittype.Naam)
            .ToList();

        // If no active besluittypen, consider it valid
        if (detBesluittypen.Count == 0)
        {
            return (true, new Dictionary<string, Uri>());
        }

        var mappings = await context.PropertyMappings
            .Where(bm => bm.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && bm.Property == "besluittype")
            .ToListAsync();

        var mappingDictionary = mappings.ToDictionary(
            x => x.SourceId,
            x => new Uri($"{_openZaakBaseUrl}catalogi/api/v1/besluittypen/{Guid.Parse(x.TargetId)}"));

        // checking if all active DET besluittypen are mapped
        var allMapped = detBesluittypen.All(mappingDictionary.ContainsKey);

        return (allMapped, mappingDictionary);
    }
}

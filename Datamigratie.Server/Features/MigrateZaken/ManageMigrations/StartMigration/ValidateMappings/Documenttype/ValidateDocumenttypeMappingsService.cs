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
    ILogger<ValidateDocumenttypeMappingsService> logger) : IValidateDocumenttypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Uri> Mappings)> ValidateAndGetDocumenttypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var zaaktypenMapping = await dbContext.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
        {
            logger.LogWarning("No zaaktype mapping found for zaaktype {DetZaaktypeFunctioneleIdentificatie}", detZaaktype.FunctioneleIdentificatie);
            return (false, new Dictionary<string, Uri>());
        }

        var documenttypen = detZaaktype.Documenttypen?.ToList() ?? [];

        if (documenttypen.Count == 0)
        {
            return (true, new Dictionary<string, Uri>());
        }

        var documenttypeMappings = await dbContext.DocumenttypeMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var missingDocumenttypen = documenttypen
            .Where(dt => !documenttypeMappings.Any(m => m.DetDocumenttypeNaam == dt.Documenttype.Naam))
            .Select(dt => dt.Documenttype.Naam)
            .ToList();

        if (missingDocumenttypen.Count != 0)
        {
            logger.LogWarning("Missing documenttype mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}. Missing {Count} documenttypes: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, missingDocumenttypen.Count, string.Join(", ", missingDocumenttypen.Take(10)));
            return (false, new Dictionary<string, Uri>());
        }

        return (true, documenttypeMappings.ToDictionary(m => m.DetDocumenttypeNaam, m => new Uri(m.OzInformatieobjecttypeUrl)));
    }
}

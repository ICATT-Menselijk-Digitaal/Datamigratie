using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documenttype;

public interface IValidateDocumenttypeMappingsService
{
    Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetDocumenttypeMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidateDocumenttypeMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateDocumenttypeMappingsService> logger) : IValidateDocumenttypeMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, string> Mappings)> ValidateAndGetDocumenttypeMappings(DetZaaktypeDetail detZaaktype)
    {
        var documenttypen = detZaaktype.Documenttypen?.ToList() ?? [];

        if (documenttypen.Count == 0)
        {
            return (true, new Dictionary<string, string>());
        }

        var documenttypeMappings = await dbContext.PropertyMappings
            .Where(m => m.Mapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "documenttype" && m.SourceId != null)
            .ToDictionaryAsync(x => x.SourceId!, m => m.TargetId);

        var missingDocumenttypen = documenttypen
            .Where(dt => !documenttypeMappings.Any(m => m.Key == dt.Documenttype.Naam))
            .Select(dt => dt.Documenttype.Naam)
            .ToList();

        if (missingDocumenttypen.Count != 0)
        {
            logger.LogWarning("Missing documenttype mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}. Missing {Count} documenttypes: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, missingDocumenttypen.Count, string.Join(", ", missingDocumenttypen.Take(10)));
            return (false, new Dictionary<string, string>());
        }

        return (true, documenttypeMappings);
    }
}

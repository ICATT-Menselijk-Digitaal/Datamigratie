using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;

public class ValidateDocumentPropertyMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidateDocumentPropertyMappingsService> logger) : IValidateDocumentPropertyMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Dictionary<string, string>> Mappings)> ValidateAndGetDocumentPropertyMappings(DetZaaktypeDetail detZaaktype)
    {
        var documentPropertyMappings = await dbContext.DocumentPropertyMappings
            .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie)
            .ToListAsync();

        // Filter out mappings with null or empty OzValue
        var validMappings = documentPropertyMappings
            .Where(m => !string.IsNullOrWhiteSpace(m.OzValue))
            .ToList();

        var publicatieNiveauMappings = validMappings
            .Where(m => m.DetPropertyName == "publicatieniveau")
            .ToList();

        var missingPublicatieNiveaus = MappingConstants.PublicatieNiveau.Options
            .Where(pn => !publicatieNiveauMappings.Any(m => m.DetValue == pn.Id))
            .ToList();

        if (missingPublicatieNiveaus.Count != 0)
        {
            logger.LogWarning("Missing publicatieniveau mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, string.Join(", ", missingPublicatieNiveaus));
            return (false, new Dictionary<string, Dictionary<string, string>>());
        }

        var documenttypen = detZaaktype.Documenttypen?.ToList() ?? [];

        if (documenttypen.Count != 0)
        {
            var documenttypeMappings = validMappings
                .Where(m => m.DetPropertyName == "documenttype")
                .ToList();

            var missingDocumenttypen = documenttypen
                .Where(dt => !documenttypeMappings.Any(m => m.DetValue == dt.Documenttype.Naam))
                .Select(dt => dt.Documenttype.Naam)
                .ToList();

            if (missingDocumenttypen.Count != 0)
            {
                logger.LogWarning("Missing documenttype mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}. Missing {Count} documenttypes: {MissingValues}",
                    detZaaktype.FunctioneleIdentificatie, missingDocumenttypen.Count, string.Join(", ", missingDocumenttypen.Take(10)));
                return (false, new Dictionary<string, Dictionary<string, string>>());
            }
        }

        var result = validMappings
            .GroupBy(m => m.DetPropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(m => m.DetValue, m => m.OzValue)
            );

        return (true, result);
    }
}

using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.DocumentPropertyMapping.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;

public class ValidateDocumentPropertyMappingsService(
    DatamigratieDbContext dbContext,
    IDetApiClient detApiClient,
    ILogger<ValidateDocumentPropertyMappingsService> logger) : IValidateDocumentPropertyMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, Dictionary<string, string>> Mappings)> ValidateAndGetDocumentPropertyMappings(DetZaaktypeDetail detZaaktype)
    {
        var zaaktypenMapping = await dbContext.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
        {
            logger.LogWarning("No zaaktypen mapping found for DET zaaktype with FunctioneleIdentificatie '{DetZaaktypeFunctioneleIdentificatie}'", 
                detZaaktype.FunctioneleIdentificatie);
            return (false, new Dictionary<string, Dictionary<string, string>>());
        }

        var documentPropertyMappings = await dbContext.DocumentPropertyMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        // Filter out mappings with null or empty OzValue
        var validMappings = documentPropertyMappings
            .Where(m => !string.IsNullOrWhiteSpace(m.OzValue))
            .ToList();

        var publicatieNiveauMappings = validMappings
            .Where(m => m.DetPropertyName == "publicatieniveau")
            .ToList();

        var missingPublicatieNiveaus = PublicatieNiveauConstants.Values
            .Where(pn => !publicatieNiveauMappings.Any(m => m.DetValue == pn))
            .ToList();

        if (missingPublicatieNiveaus.Count != 0)
        {
            logger.LogWarning("Missing publicatieniveau mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, string.Join(", ", missingPublicatieNiveaus));
            return (false, new Dictionary<string, Dictionary<string, string>>());
        }

        var allDocumenttypen = await detApiClient.GetAllDocumenttypen();
        var activeDocumenttypen = allDocumenttypen.Where(dt => dt.Actief).ToList();

        if (activeDocumenttypen.Count != 0)
        {
            var documenttypeMappings = validMappings
                .Where(m => m.DetPropertyName == "documenttype")
                .ToList();

            var missingDocumenttypen = activeDocumenttypen
                .Where(dt => !documenttypeMappings.Any(m => m.DetValue == dt.Naam))
                .Select(dt => dt.Naam)
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

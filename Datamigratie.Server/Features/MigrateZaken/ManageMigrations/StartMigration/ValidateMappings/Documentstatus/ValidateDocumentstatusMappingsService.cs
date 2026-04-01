using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;

public interface IValidateDocumentstatusMappingsService
{
    Task<(bool IsValid, Dictionary<string, DocumentStatus> Mappings)> ValidateAndGetDocumentstatusMappings();
}

public class ValidateDocumentstatusMappingsService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient) : IValidateDocumentstatusMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, DocumentStatus> Mappings)> ValidateAndGetDocumentstatusMappings()
    {
        // Get all document statuses from DET
        var allDetDocumentstatuses = await detApiClient.GetAllDocumentstatussen();
        var detDocumentstatusNames = allDetDocumentstatuses.Select(s => s.Naam).ToList();

        // If no document statuses exist in DET, consider it valid
        if (detDocumentstatusNames.Count == 0)
        {
            return (true, new Dictionary<string, DocumentStatus>());
        }

        var rawMappings = await context.PropertyMappings
            .Where(x => x.Property == "documentstatus")
            .ToDictionaryAsync(m => m.SourceId, m => m.TargetId);

        // validate all mapped OZ statuses are in DocumentStatus enum
        var invalidMappings = rawMappings
            .Where(m => !Enum.TryParse<DocumentStatus>(m.Value, out _))
            .ToList();

        if (invalidMappings.Count > 0)
        {
            var invalidDetails = string.Join(", ", invalidMappings.Select(m => $"'{m.Key}' -> '{m.Value}'"));
            var validValues = string.Join(", ", Enum.GetNames(typeof(DocumentStatus)));
            throw new InvalidOperationException(
                $"Ongeldige OpenZaak documentstatussen gevonden in mappings: {invalidDetails}. " +
                $"Geldige waarden zijn: {validValues}");
        }

        var parsedMappings = rawMappings.ToDictionary(
            m => m.Key,
            m => Enum.Parse<DocumentStatus>(m.Value));

        // checking if all DET document statuses are mapped
        var allMapped = detDocumentstatusNames.All(parsedMappings.ContainsKey);

        return (allMapped, parsedMappings);
    }
}

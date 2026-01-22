using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;

public interface IValidateDocumentPropertyMappingsService
{
    Task<(bool IsValid, Dictionary<string, Dictionary<string, string>> Mappings)> ValidateAndGetDocumentPropertyMappings(DetZaaktypeDetail detZaaktype);
}

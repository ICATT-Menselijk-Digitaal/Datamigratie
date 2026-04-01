using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Server.Constants;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PublicatieNiveau;

public interface IValidatePublicatieNiveauMappingsService
{
    Task<(bool IsValid, Dictionary<string, DocumentVertrouwelijkheidaanduiding> Mappings)> ValidateAndGetPublicatieNiveauMappings(DetZaaktypeDetail detZaaktype);
}

public class ValidatePublicatieNiveauMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<ValidatePublicatieNiveauMappingsService> logger) : IValidatePublicatieNiveauMappingsService
{
    public async Task<(bool IsValid, Dictionary<string, DocumentVertrouwelijkheidaanduiding> Mappings)> ValidateAndGetPublicatieNiveauMappings(DetZaaktypeDetail detZaaktype)
    {
        var zaaktypenMapping = await dbContext.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (zaaktypenMapping == null)
        {
            logger.LogWarning("No zaaktype mapping found for zaaktype {DetZaaktypeFunctioneleIdentificatie}", detZaaktype.FunctioneleIdentificatie);
            return (false, new Dictionary<string, DocumentVertrouwelijkheidaanduiding>());
        }

        var publicatieNiveauMappings = await dbContext.PublicatieNiveauMappings
            .Where(m => m.ZaaktypenMappingId == zaaktypenMapping.Id)
            .ToListAsync();

        var missingPublicatieNiveaus = MappingConstants.PublicatieNiveau.Options
            .Where(pn => !publicatieNiveauMappings.Any(m => m.DetPublicatieNiveau == pn.Id))
            .ToList();

        if (missingPublicatieNiveaus.Count != 0)
        {
            logger.LogWarning("Missing publicatieniveau mappings for zaaktype {DetZaaktypeFunctioneleIdentificatie}: {MissingValues}",
                detZaaktype.FunctioneleIdentificatie, string.Join(", ", missingPublicatieNiveaus.Select(p => p.Id)));
            return (false, new Dictionary<string, DocumentVertrouwelijkheidaanduiding>());
        }

        var invalidMappings = publicatieNiveauMappings
            .Where(m => !Enum.TryParse<DocumentVertrouwelijkheidaanduiding>(m.Value, true, out _))
            .ToList();

        if (invalidMappings.Count > 0)
        {
            var invalidDetails = string.Join(", ", invalidMappings.Select(m => $"'{m.Key}' -> '{m.Value}'"));
            var validValues = string.Join(", ", Enum.GetNames(typeof(DocumentVertrouwelijkheidaanduiding)));
            throw new InvalidOperationException(
                $"Ongeldige OpenZaak vertrouwelijkheidaanduidingen gevonden in mappings: {invalidDetails}. " +
                $"Geldige waarden zijn: {validValues}");
        }

        var parsedMappings = publicatieNiveauMappings.ToDictionary(
            m => m.DetPublicatieNiveau,
            m => Enum.Parse<DocumentVertrouwelijkheidaanduiding>(m.OzVertrouwelijkheidaanduiding, true));

        return (true, parsedMappings);
    }
}

using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.DocumentProperty;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
using Datamigratie.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;

public interface IBuildMigrationQueueItemService
{
    Task<MigrationQueueItem> BuildAsync(string detZaaktypeId, MigrationType migrationType);
}

public class BuildMigrationQueueItemService(
    DatamigratieDbContext dbContext,
    IDetApiClient detApiClient,
    IValidateStatusMappingsService validateStatusMappingsService,
    IValidateResultaattypeMappingsService validateResultaattypeMappingsService,
    IValidateDocumentstatusMappingsService validateDocumentstatusMappingsService,
    IValidateDocumentPropertyMappingsService validateDocumentPropertyMappingsService,
    IValidateVertrouwelijkheidMappingsService validateVertrouwelijkheidMappingsService,
    IValidateBesluittypeMappingsService validateBesluittypeMappingsService,
    IValidatePdfInformatieobjecttypeMappingService validatePdfInformatieobjecttypeMappingService,
    ILogger<BuildMigrationQueueItemService> logger) : IBuildMigrationQueueItemService
{
    public async Task<MigrationQueueItem> BuildAsync(string detZaaktypeId, MigrationType migrationType)
    {
        var detZaaktype = await detApiClient.GetZaaktypeDetail(detZaaktypeId)
            ?? throw new InvalidOperationException($"DET Zaaktype '{detZaaktypeId}' not found.");

        await ValidateZaaktypeMappingAsync(detZaaktype);

        var rsinMapping = await GetRsinMappingAsync();
        var statusMappings = await GetStatusMappingsAsync(detZaaktype);
        var resultaatMappings = await GetResultaattypeMappingsAsync(detZaaktype);
        var documentstatusMappings = await GetDocumentstatusMappingsAsync();
        var documentPropertyMappings = await GetDocumentPropertyMappingsAsync(detZaaktype);
        var vertrouwelijkheidMappings = await GetVertrouwelijkheidMappingsAsync(detZaaktype);
        var besluittypeMappings = await GetBesluittypeMappingsAsync(detZaaktype);
        var pdfInformatieobjecttypeId = await GetPdfInformatieobjecttypeIdAsync(detZaaktype);

        return new MigrationQueueItem
        {
            DetZaaktypeId = detZaaktypeId,
            MigrationType = migrationType,
            RsinMapping = rsinMapping,
            StatusMappings = statusMappings,
            ResultaatMappings = resultaatMappings,
            DocumentstatusMappings = documentstatusMappings,
            DocumentPropertyMappings = documentPropertyMappings,
            ZaakVertrouwelijkheidMappings = vertrouwelijkheidMappings,
            BesluittypeMappings = besluittypeMappings,
            PdfInformatieobjecttypeId = pdfInformatieobjecttypeId
        };
    }

    private async Task ValidateZaaktypeMappingAsync(DetZaaktypeDetail detZaaktype)
    {
        var exists = await dbContext.Mappings
            .AnyAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie);

        if (!exists)
            throw new InvalidOperationException("No zaaktype mapping found. Please configure the zaaktype mapping first.");
    }

    private async Task<RsinMapping> GetRsinMappingAsync()
    {
        var rsinMapping = await dbContext.RsinConfigurations
            .Select(x => new RsinMapping { Rsin = x.Rsin! })
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Geen rsin configuratie gevonden.");

        RsinValidator.ValidateRsin(rsinMapping.Rsin, logger);

        return rsinMapping;
    }

    private async Task<Dictionary<string, Guid>> GetStatusMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateStatusMappingsService.ValidateAndGetStatusMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first.");
    }

    private async Task<Dictionary<string, Guid>> GetResultaattypeMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateResultaattypeMappingsService.ValidateAndGetResultaattypeMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET Resultaattypen have been mapped to OZ resultaattypen. Please configure resultaattypen mappings first.");
    }

    private async Task<Dictionary<string, string>> GetDocumentstatusMappingsAsync()
    {
        var (isValid, mappings) = await validateDocumentstatusMappingsService.ValidateAndGetDocumentstatusMappings();
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET document statuses have been mapped to OZ document statuses. Please configure document status mappings first.");
    }

    private async Task<Dictionary<string, Dictionary<string, string>>> GetDocumentPropertyMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateDocumentPropertyMappingsService.ValidateAndGetDocumentPropertyMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all document properties have been mapped. Please configure publicatieniveau and documenttype mappings first.");
    }

    private async Task<Dictionary<bool, ZaakVertrouwelijkheidaanduiding>> GetVertrouwelijkheidMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateVertrouwelijkheidMappingsService.ValidateAndGetVertrouwelijkheidMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all vertrouwelijkheid values have been mapped. Please configure vertrouwelijkheid mappings first.");
    }

    private async Task<Dictionary<string, Guid>> GetBesluittypeMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateBesluittypeMappingsService.ValidateAndGetBesluittypeMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET besluittypen have been mapped to OZ besluittypen. Please configure besluittype mappings first.");
    }

    private async Task<Guid> GetPdfInformatieobjecttypeIdAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, id) = await validatePdfInformatieobjecttypeMappingService.ValidateAndGetPdfInformatieobjecttypeMapping(detZaaktype);
        return isValid && id is not null ? id.Value
            : throw new InvalidOperationException("No informatieobjecttype has been configured for the generated PDF. Please configure the PDF informatieobjecttype mapping first.");
    }
}

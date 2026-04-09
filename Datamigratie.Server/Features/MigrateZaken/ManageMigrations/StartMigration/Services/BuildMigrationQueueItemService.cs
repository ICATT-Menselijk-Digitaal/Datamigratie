using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.Extensions.Options;
using Datamigratie.Data;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Besluittype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documentstatus;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Documenttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PublicatieNiveau;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Resultaat;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Status;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Vertrouwelijkheid;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

public interface IBuildMigrationQueueItemService
{
    Task<MigrationQueueItem> ValidateAndBuildAsync(string detZaaktypeId, IZakenSelector zakenSelector);
}

public class BuildMigrationQueueItemService(
    DatamigratieDbContext dbContext,
    IDetApiClient detApiClient,
    IValidateStatusMappingsService validateStatusMappingsService,
    IValidateResultaattypeMappingsService validateResultaattypeMappingsService,
    IValidateDocumentstatusMappingsService validateDocumentstatusMappingsService,
    IValidatePublicatieNiveauMappingsService validatePublicatieNiveauMappingsService,
    IValidateDocumenttypeMappingsService validateDocumenttypeMappingsService,
    IValidateVertrouwelijkheidMappingsService validateVertrouwelijkheidMappingsService,
    IValidateBesluittypeMappingsService validateBesluittypeMappingsService,
    IValidatePdfInformatieobjecttypeMappingService validatePdfInformatieobjecttypeMappingService,
    IValidateRoltypeMappingsService validateRoltypeMappingsService,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IBuildMigrationQueueItemService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;
    public async Task<MigrationQueueItem> ValidateAndBuildAsync(string detZaaktypeId, IZakenSelector zakenSelector)
    {
        var detZaaktype = await detApiClient.GetZaaktypeDetail(detZaaktypeId)
            ?? throw new InvalidOperationException($"DET Zaaktype '{detZaaktypeId}' not found.");

        var ozZaaktypeId = await GetOzZaaktypeIdAsync(detZaaktype);

        var rsin = await GetRsinAsync();
        var ozZaaktypeUrl = new Uri($"{_openZaakBaseUrl}catalogi/api/v1/zaaktypen/{ozZaaktypeId}");
        var statusMapper = new StatusMapper(await GetStatusMappingsAsync(detZaaktype));
        var resultaatMapper = new ResultaatMapper(await GetResultaattypeMappingsAsync(detZaaktype));
        var zaakMapper = new ZaakMapper(rsin, ozZaaktypeUrl, await GetVertrouwelijkheidMappingsAsync(detZaaktype));
        var documentMapper = new DocumentMapper(
            rsin,
            await GetDocumentstatusMappingsAsync(),
            await GetPublicatieNiveauMappingsAsync(detZaaktype),
            await GetDocumenttypeMappingsAsync(detZaaktype));
        var besluitMapper = new BesluitMapper(rsin, await GetBesluittypeMappingsAsync(detZaaktype));
        var pdfInformatieobjecttypeUri = await GetPdfInformatieobjecttypeUriAsync(detZaaktype);
        var pdfMapper = new PdfMapper(rsin, pdfInformatieobjecttypeUri);
        var rolMapper = new RolMapper(await GetRoltypeMappingsAsync(detZaaktypeId));

        return new MigrationQueueItem
        {
            DetZaaktypeId = detZaaktypeId,
            ZakenSelector = zakenSelector,
            ResultaatMapper = resultaatMapper,
            StatusMapper = statusMapper,
            ZaakMapper = zaakMapper,
            DocumentMapper = documentMapper,
            BesluitMapper = besluitMapper,
            PdfMapper = pdfMapper,
            RolMapper = rolMapper
        };
    }

    private async Task<Guid> GetOzZaaktypeIdAsync(DetZaaktypeDetail detZaaktype)
    {
        var mapping = await dbContext.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie)
            ?? throw new InvalidOperationException("No zaaktype mapping found. Please configure the zaaktype mapping first.");

        return mapping.OzZaaktypeId;
    }

    private async Task<string> GetRsinAsync()
    {
        var rsin = await dbContext.PropertyMappings
            .Where(x=> x.Property == "rsin")
            .Select(x => x.TargetId)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Geen rsin configuratie gevonden.");

        RsinValidator.ValidateRsin(rsin);

        return rsin;
    }

    private async Task<Dictionary<string, Uri>> GetStatusMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateStatusMappingsService.ValidateAndGetStatusMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET statuses have been mapped to OZ statuses. Please configure status mappings first.");
    }

    private async Task<Dictionary<string, Uri>> GetResultaattypeMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateResultaattypeMappingsService.ValidateAndGetResultaattypeMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET Resultaattypen have been mapped to OZ resultaattypen. Please configure resultaattypen mappings first.");
    }

    private async Task<Dictionary<string, DocumentStatus>> GetDocumentstatusMappingsAsync()
    {
        var (isValid, mappings) = await validateDocumentstatusMappingsService.ValidateAndGetDocumentstatusMappings();
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET document statuses have been mapped to OZ document statuses. Please configure document status mappings first.");
    }

    private async Task<Dictionary<string, DocumentVertrouwelijkheidaanduiding>> GetPublicatieNiveauMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validatePublicatieNiveauMappingsService.ValidateAndGetPublicatieNiveauMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all publicatieniveaus have been mapped. Please configure publicatieniveau mappings first.");
    }

    private async Task<Dictionary<string, Uri>> GetDocumenttypeMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateDocumenttypeMappingsService.ValidateAndGetDocumenttypeMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all documenttypes have been mapped. Please configure documenttype mappings first.");
    }

    private async Task<Dictionary<bool, ZaakVertrouwelijkheidaanduiding>> GetVertrouwelijkheidMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateVertrouwelijkheidMappingsService.ValidateAndGetVertrouwelijkheidMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all vertrouwelijkheid values have been mapped. Please configure vertrouwelijkheid mappings first.");
    }

    private async Task<Dictionary<string, Uri>> GetBesluittypeMappingsAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, mappings) = await validateBesluittypeMappingsService.ValidateAndGetBesluittypeMappings(detZaaktype);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET besluittypen have been mapped to OZ besluittypen. Please configure besluittype mappings first.");
    }

    private async Task<Uri> GetPdfInformatieobjecttypeUriAsync(DetZaaktypeDetail detZaaktype)
    {
        var (isValid, url) = await validatePdfInformatieobjecttypeMappingService.ValidateAndGetPdfInformatieobjecttypeMapping(detZaaktype);
        return isValid && url is not null ? url
            : throw new InvalidOperationException("No informatieobjecttype has been configured for the generated PDF. Please configure the PDF informatieobjecttype mapping first.");
    }

    private async Task<Dictionary<DetRolType, Uri>> GetRoltypeMappingsAsync(string detZaaktypeId)
    {
        var (isValid, mappings) = await validateRoltypeMappingsService.ValidateAndGetRoltypeMappings(detZaaktypeId);
        return isValid ? mappings
            : throw new InvalidOperationException("Not all DET rollen have been mapped to OZ roltypen. Please configure roltype mappings first.");
    }
}

using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;

public interface IValidatePdfInformatieobjecttypeMappingService
{
    Task<(bool IsValid, Uri? OzInformatieobjecttypeUri)> ValidateAndGetPdfInformatieobjecttypeMapping(DetZaaktypeDetail detZaaktype);
}

public class ValidatePdfInformatieobjecttypeMappingService(
    DatamigratieDbContext context,
    IOptions<OpenZaakApiOptions> openZaakOptions) : IValidatePdfInformatieobjecttypeMappingService
{
    private readonly string _openZaakBaseUrl = openZaakOptions.Value.BaseUrl;

    public async Task<(bool IsValid, Uri? OzInformatieobjecttypeUri)> ValidateAndGetPdfInformatieobjecttypeMapping(DetZaaktypeDetail detZaaktype)
    {
        var mapping = await context.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "documenttype" && m.SourceId == "export-pdf")
            .Select(m => m.TargetId)
            .FirstOrDefaultAsync();


        return mapping is null
            ? (false, null)
            : (true, new Uri(mapping));
    }
}

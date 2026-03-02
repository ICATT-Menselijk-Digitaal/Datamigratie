using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.PdfInformatieobjecttype;

public interface IValidatePdfInformatieobjecttypeMappingService
{
    Task<(bool IsValid, Guid? OzInformatieobjecttypeId)> ValidateAndGetPdfInformatieobjecttypeMapping(DetZaaktypeDetail detZaaktype);
}

public class ValidatePdfInformatieobjecttypeMappingService(
    DatamigratieDbContext context) : IValidatePdfInformatieobjecttypeMappingService
{
    public async Task<(bool IsValid, Guid? OzInformatieobjecttypeId)> ValidateAndGetPdfInformatieobjecttypeMapping(DetZaaktypeDetail detZaaktype)
    {
        var mapping = await context.PdfInformatieobjecttypeMappings
            .Where(m => m.ZaaktypenMapping.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie)
            .FirstOrDefaultAsync();

        return mapping is null
            ? (false, null)
            : (true, mapping.OzInformatieobjecttypeId);
    }
}

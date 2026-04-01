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
        var mapping = await context.PropertyMappings
            .Where(m => m.ZaaktypenMapping!.DetZaaktypeId == detZaaktype.FunctioneleIdentificatie && m.Property == "documenttype" && m.SourceId == "export-pdf")
            .Select(m => m.TargetId)
            .FirstOrDefaultAsync();

        var lastPathPart = mapping?.Split('/').LastOrDefault();

        return !Guid.TryParse(lastPathPart, out var uuid)
            ? (false, null)
            : (true, uuid);
    }
}

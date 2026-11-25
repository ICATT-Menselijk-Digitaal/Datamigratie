using Datamigratie.Data;
using Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.RoltypeMapping.SaveRoltypeMappings;

public interface ISaveRoltypeMappingsService
{
    Task SaveRoltypeMappings(Guid zaaktypenMappingId, SaveRoltypeMappingsRequest request);
}

public class SaveRoltypeMappingsService(DatamigratieDbContext context) : ISaveRoltypeMappingsService
{
    public async Task SaveRoltypeMappings(Guid zaaktypenMappingId, SaveRoltypeMappingsRequest request)
    {
        var existingMappings = await context.RoltypeMappings
            .Where(m => m.ZaaktypeVersieMapping.ZaaktypenMappingId == zaaktypenMappingId)
            .ToListAsync();

        context.RoltypeMappings.RemoveRange(existingMappings);

        // TODO DATA-247: take the zaaktype version from the route once mapping per version is in place
        var zaaktypeVersieMappingId = await context.ZaaktypeVersieMappings
            .Where(v => v.ZaaktypenMappingId == zaaktypenMappingId)
            .Select(v => v.Id)
            .SingleAsync();

        var newMappings = request.Mappings.Select(m => new Data.Entities.RoltypeMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetRol = m.DetRol,
            AlleenPdf = m.AlleenPdf,
            OzRoltypeUrl = m.OzRoltypeUrl
        });

        await context.RoltypeMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }
}

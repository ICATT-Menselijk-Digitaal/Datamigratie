using Datamigratie.Data;
using Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.ManageMapping.ZaaktypeMapping.ZaaktypeDetailsMapping.BesluittypeMapping.ShowBesluittypeMappings.Services;

public interface IShowBesluittypeMappingsService
{
    Task<List<BesluittypeMappingsResponse>> GetBesluittypeMappings(Guid zaaktypeVersieMappingId);
}

public class ShowBesluittypeMappingsService(DatamigratieDbContext context) : IShowBesluittypeMappingsService
{
    public async Task<List<BesluittypeMappingsResponse>> GetBesluittypeMappings(Guid zaaktypeVersieMappingId)
    {
        var existingMappings = await context.BesluittypeMappings
            .Where(bm => bm.ZaaktypeVersieMappingId == zaaktypeVersieMappingId)
            .ToListAsync();

        return [.. existingMappings
                .Select(em => new BesluittypeMappingsResponse
                {
                    DetBesluittypeNaam = em.DetBesluittypeNaam,
                    OzBesluittypeId = em.OzBesluittypeId
                })];
    }
}

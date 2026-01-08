using Datamigratie.Common.Services.Det;
using Datamigratie.Data;
using Datamigratie.Server.Features.Mapping.StatusMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Mapping.StatusMapping.Services;

public interface IStatusMappingService
{
    Task<StatusMappingsResponse> GetStatusMappings(string detZaaktypeId);
    Task SaveStatusMappings(SaveStatusMappingsRequest request);
    Task<bool> AreAllStatusesMapped(string detZaaktypeId);
}

public class StatusMappingService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient) : IStatusMappingService
{
    public async Task<StatusMappingsResponse> GetStatusMappings(string detZaaktypeId)
    {
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.DetZaaktypeId == detZaaktypeId)
            .ToListAsync();

        return new StatusMappingsResponse
        {
            Mappings = existingMappings
                .Select(em => new StatusMappingDto
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })
                .ToList()
        };
    }

    public async Task SaveStatusMappings(SaveStatusMappingsRequest request)
    {        
        var existingMappings = await context.StatusMappings
            .Where(sm => sm.DetZaaktypeId == request.DetZaaktypeId)
            .ToListAsync();
        
        context.StatusMappings.RemoveRange(existingMappings);

        var newMappings = request.Mappings.Select(m => new Data.Entities.StatusMapping
        {
            DetZaaktypeId = request.DetZaaktypeId,
            DetStatusNaam = m.DetStatusNaam,
            OzStatustypeId = m.OzStatustypeId,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        });
        
        await context.StatusMappings.AddRangeAsync(newMappings);
        await context.SaveChangesAsync();
    }

    public async Task<bool> AreAllStatusesMapped(string detZaaktypeId)
    {
        var detZaaktype = await detApiClient.GetZaaktypeDetail(detZaaktypeId);
        
        if (detZaaktype == null)
            return false;

        var activeDetStatuses = detZaaktype.Statussen
            .Where(s => s.Actief)
            .Select(s => s.Naam)
            .ToList();

        if (activeDetStatuses.Count == 0)
            return true;

        var mappedStatuses = await context.StatusMappings
            .Where(sm => sm.DetZaaktypeId == detZaaktypeId)
            .Select(sm => sm.DetStatusNaam)
            .ToListAsync();

        // checking if all active DET statuses are mapped
        return activeDetStatuses.All(status => mappedStatuses.Contains(status));
    }
}

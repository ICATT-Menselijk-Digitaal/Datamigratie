using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Data;
using Datamigratie.Server.Features.StatusMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.StatusMapping.Services;

public interface IStatusMappingService
{
    Task<StatusMappingsResponse> GetStatusMappingsForZaaktype(string detZaaktypeId);
    Task SaveStatusMappings(SaveStatusMappingsRequest request);
    Task<bool> AreAllStatusesMapped(string detZaaktypeId);
}

public class StatusMappingService(
    DatamigratieDbContext context,
    IDetApiClient detApiClient,
    IOpenZaakApiClient openZaakApiClient) : IStatusMappingService
{
    public async Task<StatusMappingsResponse> GetStatusMappingsForZaaktype(string detZaaktypeId)
    {
        var mapping = await context.Mappings
            .FirstOrDefaultAsync(m => m.DetZaaktypeId == detZaaktypeId)
            ?? throw new InvalidOperationException($"No mapping found for DetZaaktypeId {detZaaktypeId}");

        var detZaaktype = await detApiClient.GetZaaktypeDetail(detZaaktypeId)
            ?? throw new InvalidOperationException($"DET Zaaktype {detZaaktypeId} not found");

        var ozZaaktype = await openZaakApiClient.GetZaaktype(mapping.OzZaaktypeId)
            ?? throw new InvalidOperationException($"OZ Zaaktype {mapping.OzZaaktypeId} not found");

        var ozStatustypes = await openZaakApiClient.GetStatustypesForZaaktype(new Uri(ozZaaktype.Url));

        var existingMappings = await context.StatusMappings
            .Where(sm => sm.DetZaaktypeId == detZaaktypeId)
            .ToListAsync();

        return new StatusMappingsResponse
        {
            DetStatuses = [.. detZaaktype.Statussen
                .Where(s => s.Actief)
                .Select(s => new DetStatusDto
                {
                    Naam = s.Naam,
                    Omschrijving = s.Omschrijving,
                    Eind = s.Eind
                })],
            OzStatustypes = [.. ozStatustypes
                .OrderBy(st => st.Volgnummer)
                .Select(st => new OzStatustypeDto
                {
                    Uuid = st.Uuid,
                    Omschrijving = st.Omschrijving,
                    Volgnummer = st.Volgnummer,
                    IsEindstatus = st.IsEindstatus
                })],
            ExistingMappings = [.. existingMappings
                .Select(em => new StatusMappingDto
                {
                    DetStatusNaam = em.DetStatusNaam,
                    OzStatustypeId = em.OzStatustypeId
                })]
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

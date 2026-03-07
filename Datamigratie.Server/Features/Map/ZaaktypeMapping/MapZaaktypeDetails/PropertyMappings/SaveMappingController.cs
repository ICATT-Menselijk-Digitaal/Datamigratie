using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PropertyMappings;

[ApiController]
public class SaveMappingController(DatamigratieDbContext db) : ControllerBase
{
    [HttpPut("api/mappings/properties/{property}/{mappingId:guid?}")]
    public async Task<IActionResult> Put(
        string property,
        Guid? mappingId,
        [FromBody] PropertyMappingModel[] mappings,
        CancellationToken token)
    {
        await db.PropertyMappings
            .Where(x => x.MappingId == mappingId && x.Property == property)
            .ExecuteDeleteAsync(token);

        var entities = mappings.Select(m => new PropertyMapping
        {
            Id = Guid.NewGuid(),
            MappingId = mappingId,
            Property = property,
            SourceId = m.SourceId,
            TargetId = m.TargetId
        });

        await db.PropertyMappings
            .AddRangeAsync(entities, token);

        await db.SaveChangesAsync(token);

        return NoContent();
    }
}

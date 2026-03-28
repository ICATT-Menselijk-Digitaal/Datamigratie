using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PropertyMappings;

[ApiController]
public class GetMappingController(DatamigratieDbContext db) : ControllerBase
{
    [HttpGet("api/mappings/properties/{property}/{mappingId:guid?}")]
    public IActionResult Get(
        string property,
        Guid? mappingId
        )
    {
        var result = db.PropertyMappings
            .Where(x => x.MappingId == mappingId && x.Property == property)
            .Select(x => new PropertyMappingModel(x.SourceId, x.TargetId))
            .AsAsyncEnumerable();

        return Ok(result);
    }
}

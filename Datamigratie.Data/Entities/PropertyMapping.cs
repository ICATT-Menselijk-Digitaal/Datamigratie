using System;

namespace Datamigratie.Data.Entities;

public class PropertyMapping
{
    public required Guid Id { get; init; }
    public Guid? MappingId { get; init; }
    public required string Property { get; init; }
    public string? SourceId { get; init; }
    public required string TargetId { get; init; }
    public ZaaktypenMapping? Mapping { get; init; }
}

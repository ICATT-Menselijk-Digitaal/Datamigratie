using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.PropertyMappings;

public record PropertyMappingModel(string? SourceId, [Required] string TargetId);

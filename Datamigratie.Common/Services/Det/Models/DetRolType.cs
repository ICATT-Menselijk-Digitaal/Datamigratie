using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.Det.Models;

/// <summary>
/// The set of DET rol types that can be mapped to OpenZaak roltypes.
/// The string representation of each value (via <see cref="object.ToString"/>) matches the
/// value stored in <c>RoltypeMapping.DetRol</c> and used as a dictionary key in the migration pipeline.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DetRolType>))]
public enum DetRolType
{
    initiator,
    behandelaar,
    belanghebbende,
    gemachtigde,
    melder,
    medeaanvrager,
    plaatsvervanger,
    overig,
}

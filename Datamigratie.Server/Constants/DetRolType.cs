namespace Datamigratie.Server.Constants;

/// <summary>
/// The set of DET rol types that can be mapped to OpenZaak roltypes.
/// The string representation of each value (via <see cref="object.ToString"/>) matches the
/// value stored in <c>RoltypeMapping.DetRol</c> and used as a dictionary key in the migration pipeline.
/// </summary>
public enum DetRolType
{
    Initiator,
    Behandelaar,
    Belanghebbende,
    Gemachtigde,
    Melder,
    Medeaanvrager,
    Plaatsvervanger,
    Overig,
}

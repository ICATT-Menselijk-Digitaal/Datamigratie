namespace Datamigratie.Common.Services.Selectielijst.Models;

/// <summary>
/// A procestype from the Selectielijst API. Each selectielijst year has its own set of procestypen.
/// </summary>
public class SelectielijstProcestype
{
    public required string Url { get; set; }

    /// <summary>
    /// The domain number, e.g. 6 for procestype 6 "Toezicht houden".
    /// </summary>
    public required int Nummer { get; set; }

    /// <summary>
    /// The selectielijst year this procestype belongs to, e.g. 2017 or 2020.
    /// </summary>
    public required int Jaar { get; set; }

    public required string Naam { get; set; }
}

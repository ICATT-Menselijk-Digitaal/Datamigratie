namespace Datamigratie.Common.Services.Selectielijst.Models;

/// <summary>
/// A resultaat (selectielijstklasse) from the Selectielijst API.
/// </summary>
public class SelectielijstResultaat
{
    public required string Url { get; set; }

    /// <summary>
    /// URL of the procestype this resultaat belongs to.
    /// </summary>
    public required string ProcesType { get; set; }

    /// <summary>
    /// The full number including the procestype, e.g. "6.1".
    /// </summary>
    public required string VolledigNummer { get; set; }

    public required string Naam { get; set; }
}

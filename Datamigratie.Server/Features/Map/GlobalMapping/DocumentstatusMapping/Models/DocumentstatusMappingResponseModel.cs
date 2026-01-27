namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;

/// <summary>
/// Response model for document status mappings
/// </summary>
public class DocumentstatusMappingResponseModel
{
    /// <summary>
    /// The DET (e-Suite) document status name
    /// </summary>
    public required string DetDocumentstatus { get; set; }

    /// <summary>
    /// The OpenZaak document status.
    /// Valid values: "in_bewerking", "ter_vaststelling", "definitief", "gearchiveerd"
    /// </summary>
    public required string OzDocumentstatus { get; set; }
}

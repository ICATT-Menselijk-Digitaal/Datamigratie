namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;

/// <summary>
/// Request model for saving document status mappings
/// </summary>
public class SaveDocumentstatusMappingsRequest
{
    /// <summary>
    /// List of document status mappings to save
    /// </summary>
    public required List<DocumentstatusMappingItem> Mappings { get; set; }
}

/// <summary>
/// Represents a single document status mapping
/// </summary>
public class DocumentstatusMappingItem
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

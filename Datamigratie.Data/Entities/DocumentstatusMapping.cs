using System.ComponentModel.DataAnnotations;

namespace Datamigratie.Data.Entities;

/// <summary>
/// Global mapping between e-Suite (DET) document statuses and OpenZaak document statuses.
/// This mapping is shared across all document types.
/// </summary>
public class DocumentstatusMapping
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The DET document status name (from e-Suite)
    /// </summary>
    [Required]
    public required string DetDocumentstatus { get; set; }

    /// <summary>
    /// The corresponding OpenZaak document status.
    /// Valid values: "in_bewerking", "ter_vaststelling", "definitief", "gearchiveerd"
    /// </summary>
    [Required]
    public required string OzDocumentstatus { get; set; }
}

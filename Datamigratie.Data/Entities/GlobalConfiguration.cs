namespace Datamigratie.Data.Entities;

/// <summary>
/// RSIN configuration settings for the municipality
/// </summary>
public class RsinConfiguration
{
    /// <summary>
    /// Configuration key identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) of the municipality
    /// Must be 9 characters and pass the 11-test
    /// </summary>
    public string? Rsin { get; set; }

    /// <summary>
    /// When the configuration was last updated 
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

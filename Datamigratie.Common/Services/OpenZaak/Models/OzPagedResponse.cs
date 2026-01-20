using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models;

/// <summary>
/// Represents a paginated response from the OpenZaak API.
/// OpenZaak API uses URL strings for pagination.
/// </summary>
/// <typeparam name="T">The type of the objects in the results list.</typeparam>
public class OzPagedResponse<T>
{
    public int Count { get; set; }
    
    [JsonPropertyName("next")]
    public string? Next { get; set; }
    
    [JsonPropertyName("previous")]
    public string? Previous { get; set; }
    
    public required List<T> Results { get; set; }
    
    [JsonIgnore]
    public bool NextPage => !string.IsNullOrEmpty(Next);
    
    [JsonIgnore]
    public bool PreviousPage => !string.IsNullOrEmpty(Previous);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Datamigratie.Common.Services.Shared.Models
{
    /// <summary>
    /// Represents a paginated response from the API.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the results list.</typeparam>
    public class PagedResponse<T>
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
}

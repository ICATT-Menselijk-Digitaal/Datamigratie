namespace Datamigratie.Common.Services.Det.Models;

/// <summary>
/// Represents a paginated response from the DET API.
/// DET API uses boolean properties for pagination.
/// </summary>
/// <typeparam name="T">The type of the objects in the results list.</typeparam>
public class DetPagedResponse<T>
{
    public int Count { get; set; }
    public bool NextPage { get; set; }
    public bool PreviousPage { get; set; }
    public required List<T> Results { get; set; }
}

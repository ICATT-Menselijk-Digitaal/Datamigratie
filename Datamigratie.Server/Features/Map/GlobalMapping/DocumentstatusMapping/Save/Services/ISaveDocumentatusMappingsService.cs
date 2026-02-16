using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save.Services;

/// <summary>
/// Service for managing global document status mappings
/// </summary>
public interface ISaveDocumentstatusMappingsService
{
    /// <summary>
    /// Saves or updates the global document status mappings
    /// </summary>
    /// <param name="request">The document status mappings to save</param>
    /// <returns>The saved mappings</returns>
    Task<List<DocumentstatusMappingResponseModel>> SaveDocumentstatusMappings(
        SaveDocumentstatusMappingsRequest request);
}

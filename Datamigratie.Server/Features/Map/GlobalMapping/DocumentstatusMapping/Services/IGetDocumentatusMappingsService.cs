using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Services;

/// <summary>
/// Service for retrieving global document status mappings
/// </summary>
public interface IGetDocumentstatusMappingsService
{
    /// <summary>
    /// Retrieves all global document status mappings
    /// </summary>
    /// <returns>List of document status mappings</returns>
    Task<List<DocumentstatusMappingResponseModel>> GetDocumentstatusMappings();
}

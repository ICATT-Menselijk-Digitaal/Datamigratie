using Datamigratie.Data;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Services;

/// <summary>
/// Implementation of the get document status mappings service
/// </summary>
public class GetDocumentstatusMappingsService(
    DatamigratieDbContext dbContext) : IGetDocumentstatusMappingsService
{
    /// <summary>
    /// Retrieves all global document status mappings
    /// </summary>
    public async Task<List<DocumentstatusMappingResponseModel>> GetDocumentstatusMappings()
    {
        var mappings = await dbContext.DocumentstatusMappings
            .OrderBy(m => m.DetDocumentstatus)
            .ToListAsync();

        var result = mappings.Select(m => new DocumentstatusMappingResponseModel
        {
            DetDocumentstatus = m.DetDocumentstatus,
            OzDocumentstatus = m.OzDocumentstatus,
            UpdatedAt = m.UpdatedAt
        }).ToList();

        return result;
    }
}

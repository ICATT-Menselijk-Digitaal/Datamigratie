using Datamigratie.Data;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Show;

/// <summary>
/// Controller for retrieving global document status mappings
/// </summary>
[ApiController]
[Route("api/globalmapping/documentstatuses")]
public class GetDocumentstatusMappingsController(
    DatamigratieDbContext dbContext,
    ILogger<GetDocumentstatusMappingsController> logger) : ControllerBase
{
    /// <summary>
    /// Retrieves all global document status mappings
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DocumentstatusMappingResponseModel>>> GetDocumentstatusMappings()
    {
        try
        {           
            return await dbContext.DocumentstatusMappings
            .Select(m => new DocumentstatusMappingResponseModel
            {
                DetDocumentstatus = m.DetDocumentstatus,
                OzDocumentstatus = m.OzDocumentstatus,
            })
            .OrderBy(m => m.DetDocumentstatus)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving document status mappings");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de document status mappings.");
        }
    }
}

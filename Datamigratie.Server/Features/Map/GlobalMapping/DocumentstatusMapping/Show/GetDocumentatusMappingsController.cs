using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Show;

/// <summary>
/// Controller for retrieving global document status mappings
/// </summary>
[ApiController]
[Route("api/globalmapping/documentstatuses")]
public class GetDocumentstatusMappingsController(
    IGetDocumentstatusMappingsService getDocumentstatusMappingsService,
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
            var mappings = await getDocumentstatusMappingsService.GetDocumentstatusMappings();
            return Ok(mappings);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving document status mappings");
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de document status mappings.");
        }
    }
}

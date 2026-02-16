using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save.Services;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save;

/// <summary>
/// Controller for saving global document status mappings
/// </summary>
[ApiController]
[Route("api/globalmapping/documentstatuses")]
public class SaveDocumentstatusMappingsController(
    ISaveDocumentstatusMappingsService saveDocumentstatusMappingsService,
    ILogger<SaveDocumentstatusMappingsController> logger) : ControllerBase
{
    /// <summary>
    /// Saves the global document status mappings
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<List<DocumentstatusMappingResponseModel>>> SaveDocumentstatusMappings(
        [FromBody] SaveDocumentstatusMappingsRequest request)
    {
        try
        {
            if (request?.Mappings == null || request.Mappings.Count == 0)
            {
                return BadRequest("At least one documentstatus mapping is required.");
            }

            var result = await saveDocumentstatusMappingsService.SaveDocumentstatusMappings(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error in SaveDocumentstatusMappings");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving document status mappings");
            return StatusCode(500, "Er is een fout opgetreden bij het opslaan van de document status mappings.");
        }
    }
}

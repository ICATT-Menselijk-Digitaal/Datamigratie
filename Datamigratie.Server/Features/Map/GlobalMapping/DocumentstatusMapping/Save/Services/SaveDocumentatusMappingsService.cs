using Datamigratie.Data;
using Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Models;
using Microsoft.EntityFrameworkCore;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.Save.Services;

/// <summary>
/// Implementation of document status mapping service
/// </summary>
public class SaveDocumentstatusMappingsService(
    DatamigratieDbContext dbContext,
    ILogger<SaveDocumentstatusMappingsService> logger) : ISaveDocumentstatusMappingsService
{
    private static readonly HashSet<string> ValidOzDocumentStatuses =
    [
        "in_bewerking",
        "ter_vaststelling",
        "definitief",
        "gearchiveerd"
    ];

    /// <summary>
    /// Saves or updates the global document status mappings
    /// </summary>
    public async Task<List<DocumentstatusMappingResponseModel>> SaveDocumentstatusMappings(
        SaveDocumentstatusMappingsRequest request)
    {
        try
        {
            // Validate all mappings first
            foreach (var mapping in request.Mappings)
            {
                if (string.IsNullOrWhiteSpace(mapping.DetDocumentstatus))
                {
                    throw new ArgumentException("DET document status cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(mapping.OzDocumentstatus))
                {
                    throw new ArgumentException("OZ document status cannot be empty.");
                }

                if (!ValidOzDocumentStatuses.Contains(mapping.OzDocumentstatus))
                {
                    throw new ArgumentException(
                        $"Invalid OZ document status '{mapping.OzDocumentstatus}'.");
                }
            }

            // Delete existing mappings
            dbContext.DocumentstatusMappings.RemoveRange(await dbContext.DocumentstatusMappings.ToListAsync());

            // Add new mappings
            var now = DateTime.UtcNow;
            var newMappings = request.Mappings.Select(m => new Data.Entities.DocumentstatusMapping
            {
                DetDocumentstatus = m.DetDocumentstatus,
                OzDocumentstatus = m.OzDocumentstatus,
            }).ToList();

            dbContext.DocumentstatusMappings.AddRange(newMappings);
            await dbContext.SaveChangesAsync();

            return [.. newMappings.Select(m => new DocumentstatusMappingResponseModel
            {
                DetDocumentstatus = m.DetDocumentstatus,
                OzDocumentstatus = m.OzDocumentstatus
            })];
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation error while saving document status mappings");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving document status mappings");
            throw;
        }
    }
}

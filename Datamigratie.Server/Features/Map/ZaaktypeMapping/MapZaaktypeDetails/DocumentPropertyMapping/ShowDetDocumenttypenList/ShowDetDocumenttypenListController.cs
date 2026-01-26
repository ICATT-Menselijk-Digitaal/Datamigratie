using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.DocumentPropertyMapping.ShowDetDocumenttypenList;

[ApiController]
[Route("api/det/documenttypen")]
public class ShowDetDocumenttypenListController(IDetApiClient detApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<DetDocumenttype>> GetAllDocumenttypen()
    {
        return await detApiClient.GetAllDocumenttypen();
    }
}

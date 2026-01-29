using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.GlobalMapping.DocumentstatusMapping.ShowDetDocumentstatussen;

[ApiController]
[Route("api/det/documentstatussen")]
public class ShowDetDocumentstatussenController(IDetApiClient detApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<DetDocumentstatus>> GetAllDocumentstatussen()
    {
        return await detApiClient.GetAllDocumentstatussen();
    }
}

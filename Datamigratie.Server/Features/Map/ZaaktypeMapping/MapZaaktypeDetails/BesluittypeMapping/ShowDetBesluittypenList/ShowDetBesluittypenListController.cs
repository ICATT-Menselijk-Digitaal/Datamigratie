using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Microsoft.AspNetCore.Mvc;

namespace Datamigratie.Server.Features.Map.ZaaktypeMapping.MapZaaktypeDetails.BesluittypeMapping.ShowDetBesluittypenList;

[ApiController]
[Route("api/det/besluittypen")]
public class ShowDetBesluittypenListController(IDetApiClient detApiClient) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<DetBesluittype>> GetAllBesluittypen()
    {
        return await detApiClient.GetAllBesluittypen();
    }
}

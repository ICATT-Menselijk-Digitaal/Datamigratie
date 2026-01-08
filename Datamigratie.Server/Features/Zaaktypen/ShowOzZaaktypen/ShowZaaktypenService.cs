using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen
{

    public interface IShowOzZaaktypenService
    {
        Task<IEnumerable<OzZaaktype>> GetAllZaakTypen();
    }

    public class ShowOzZaaktypenService(IOpenZaakApiClient openZaakApiClient) : IShowOzZaaktypenService
    {
        public async Task<IEnumerable<OzZaaktype>> GetAllZaakTypen()
        {
            return await openZaakApiClient.GetAllZaakTypen();
        }
    }
}

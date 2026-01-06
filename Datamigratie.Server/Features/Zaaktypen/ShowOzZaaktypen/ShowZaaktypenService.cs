using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowOzZaaktypen
{

    public interface IShowZaaktypenService
    {
        Task<IEnumerable<OzZaaktype>> GetAllZaakTypen();
        Task<EnrichedOzZaaktype?> GetEnrichedZaaktype(Guid zaaktypeId);
    }

    public class ShowZaaktypenService(IOpenZaakApiClient openZaakApiClient) : IShowZaaktypenService
    {
        public async Task<IEnumerable<OzZaaktype>> GetAllZaakTypen()
        {
            return await openZaakApiClient.GetAllZaakTypen();
        }

        public async Task<EnrichedOzZaaktype?> GetEnrichedZaaktype(Guid zaaktypeId)
        {
            var ozZaaktype = await openZaakApiClient.GetZaaktype(zaaktypeId);

            if (ozZaaktype == null)
            {
                return null;
            }

            var ozResultaattypen = await openZaakApiClient.GetResultaattypenForZaaktype(zaaktypeId);

            var enrichedOzZaaktype = new EnrichedOzZaaktype
            {
                Url = ozZaaktype.Url,
                Identificatie = ozZaaktype.Identificatie,
                Resultaattypen = ozResultaattypen,
            };

            return enrichedOzZaaktype;
        }
    }
}

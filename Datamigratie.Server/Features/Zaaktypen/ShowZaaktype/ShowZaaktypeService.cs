using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;

namespace Datamigratie.Server.Features.Zaaktypen.ShowZaaktype
{
    public interface IShowZaaktypeService
    {
        Task<EnrichedOzZaaktype?> GetEnrichedZaaktype(Guid zaaktypeId);
    }

    public class ShowZaaktypeService(IOpenZaakApiClient openZaakApiClient) : IShowZaaktypeService
    {
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

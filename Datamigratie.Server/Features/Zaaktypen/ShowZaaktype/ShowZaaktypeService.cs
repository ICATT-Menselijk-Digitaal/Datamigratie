using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo.Models;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{

    public interface IShowZaaktypeService
    {
        Task<Zaaktype> GetZaaktype(string zaaktypeId, string zaaktypeName);
    }
    public class ShowZaaktypeService(IDetApiClient _detApiClient) : IShowZaaktypeService
    {
        public async Task<Zaaktype> GetZaaktype(string zaaktypeId, string zaaktypeName)
        {
            var detZaken = await _detApiClient.GetZakenByZaaktypeAsync(zaaktypeId);
            var detZaaktype = await _detApiClient.GetSpecificZaaktype(zaaktypeName);

            var closedDetZaken = detZaken.Count(z => !z.Open);

            var enrichedDetZaaktype = new EnrichedDetZaaktype
            {
                Naam = detZaaktype.Naam,
                Omschrijving = detZaaktype.Omschrijving,
                Actief = detZaaktype.Actief,
                FunctioneleIdentificatie = detZaaktype.FunctioneleIdentificatie,
                ClosedZaken = closedDetZaken,
            };

            var zaakType = new Zaaktype
            {
                DetZaakType = enrichedDetZaaktype
            };

            return zaakType;
        }
    }
}

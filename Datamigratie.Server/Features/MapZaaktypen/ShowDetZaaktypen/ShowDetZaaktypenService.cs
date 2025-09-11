using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Features.Zaaktypen.ShowZaaktype.Models;

namespace Datamigratie.Server.Features.Zaaktypen.GetZaaktypenInfo
{

    public interface IShowZaaktypenService
    {
        Task<EnrichedDetZaaktype> GetZaaktype(string zaaktypeId);
    }
    public class ShowDetZaaktypenService(IDetApiClient _detApiClient) : IShowZaaktypenService
    {
        public async Task<EnrichedDetZaaktype> GetZaaktype(string zaaktypeId)
        {
            // TODO -> in the future we want to fetch a single det zaaktype by id instead of fetching all and filtering
            // this is currently not supported by the det api (only by name, which is not recommended)
            var detZaaktypen = await _detApiClient.GetAllZaakTypen();

            var detZaaktype = detZaaktypen.Find(z => z.FunctioneleIdentificatie == zaaktypeId);

            var detZaken = await _detApiClient.GetZakenByZaaktypeAsync(zaaktypeId);

            var closedDetZaken = detZaken.Count(z => !z.Open);

            var enrichedDetZaaktype = new EnrichedDetZaaktype
            {
                Naam = detZaaktype.Naam,
                Omschrijving = detZaaktype.Omschrijving,
                Actief = detZaaktype.Actief,
                FunctioneleIdentificatie = detZaaktype.FunctioneleIdentificatie,
                ClosedZakenCount = closedDetZaken,
            };

            return enrichedDetZaaktype;
        }
    }
}

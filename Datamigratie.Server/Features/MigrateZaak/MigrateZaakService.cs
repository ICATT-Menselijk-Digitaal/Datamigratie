using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaak
{

    public interface IMigrateZaakService
    {
        public Task<OzZaak> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId);
    }

    public class MigrateZaakService : IMigrateZaakService
    {
        private readonly IOpenZaakApiClient _openZaakApiClient;

        private readonly OpenZaakApiOptions _openZaakApiOptions;

        public MigrateZaakService(IOpenZaakApiClient openZaakApiClient, IOptions<OpenZaakApiOptions> options)
        {
            _openZaakApiClient = openZaakApiClient;
            _openZaakApiOptions = options.Value;
        }

        public async Task<OzZaak> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId)
        {
            CheckIfZaakAlreadyExists();

            var createZaakRequest = CreateOzZaakCreationRequest(detZaak, ozZaaktypeId);
            
            var createdZaak = await _openZaakApiClient.CreateZaak(createZaakRequest);

            return createdZaak;
        }

        private static void CheckIfZaakAlreadyExists()
        {
            // TODO -> story DATA-48
        }

        private CreateOzZaakRequest CreateOzZaakCreationRequest(DetZaak detZaak, Guid ozZaaktypeId)
        {
            // First apply data transformation to follow OpenZaak constraints
            var openZaakBaseUrl = _openZaakApiOptions.BaseUrl;
            var url = $"{openZaakBaseUrl}catalogi/api/v1/zaaktypen/{ozZaaktypeId}";

            var registratieDatum = detZaak.CreatieDatumTijd.ToString("yyyy-MM-dd");

            var startDatum = detZaak.Startdatum.ToString("yyyy-MM-dd");
            
            const int MaxOmschrijvingLength = 80;
            var omschrijving = TruncateWithDots(detZaak.Omschrijving, MaxOmschrijvingLength);

            // Now create the request

            var createRequest = new CreateOzZaakRequest
            {
                Identificatie = detZaak.FunctioneleIdentificatie,
                Bronorganisatie = "999990639", // moet een valide rsin zijn
                Omschrijving = omschrijving,
                Zaaktype = url,
                VerantwoordelijkeOrganisatie = "999990639",  // moet een valide rsin zijn
                Startdatum = startDatum,

                //verplichte velden, ookal zeggen de specs van niet
                Registratiedatum = registratieDatum, //todo moet deze zijn, maar die heeft een raar format. moet custom gedeserialized worden sourceZaak.CreatieDatumTijd
                Vertrouwelijkheidaanduiding = "openbaar", //hier moet in een latere story nog custom mapping voor komen
                Betalingsindicatie = "",
                Archiefstatus = "nog_te_archiveren"
            };

            return createRequest;
        }

        /// <summary>
        /// Truncates the string when the length of the input string exceeds the maxLength
        /// If this happen three dots are added to the end to indiciate that the orignal value was truncated
        /// 
        /// The maxLength param will be the length of the string with dots
        /// Example: input: [hello world], maxlength[5] -> output: he... [length=5]
        /// </summary>
        private static string TruncateWithDots(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length <= maxLength)
                return input;

            var dots = "...";

            // edge case if the max length is equal or smaller than the size of the dots
            // this would not happen unless the allowed input is really tiny, but lets return the dots just incase
            // so we can safely substract dots.length from maxLength later without it becoming negative
            if (dots.Length >= maxLength)
            {
                return dots;
            }

            var truncatedInput = input.Substring(0, maxLength - dots.Length).TrimEnd();

            return truncatedInput + dots;
        }

    }



}

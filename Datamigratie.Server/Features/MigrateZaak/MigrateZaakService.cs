using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaak
{

    public interface IMigrateZaakService
    {
        public Task<OzZaak> MigrateZaak(DetZaak detZaak, OzZaaktype ozZaaktype);
    }

    public class MigrateZaakService(IOpenZaakApiClient openZaakApiClient) : IMigrateZaakService
    {
        public async Task<OzZaak> MigrateZaak(DetZaak detZaak, OzZaaktype ozZaaktype)
        {
            CheckIfZaakAlreadyExists();

            var createZaakRequest = CreateOzZaakCreationRequest(detZaak, ozZaaktype);
            
            var createdZaak = await openZaakApiClient.CreateZaak(createZaakRequest);

            return createdZaak;
        }

        private static void CheckIfZaakAlreadyExists()
        {
            // TODO -> story DATA-48
        }

        private static CreateOzZaakRequest CreateOzZaakCreationRequest(DetZaak detZaak, OzZaaktype ozZaaktype)
        {
            // First apply data transformation to follow OpenZaak constraints

            if (detZaak.Omschrijving.Length > 80)
            {
                detZaak.Omschrijving = Truncate(detZaak.Omschrijving, 80);
            }

            var registratieDatum = ConvertNamedTimezoneToDateTime(detZaak.CreatieDatumTijd).ToString("yyyy-MM-dd");

            var startDatum = detZaak.Startdatum.ToString("yyyy-MM-dd");

            // Now create the request

            var createRequest = new CreateOzZaakRequest
            {
                Identificatie = detZaak.FunctioneleIdentificatie,
                Bronorganisatie = "999990639", // moet een valide rsin zijn
                Omschrijving = detZaak.Omschrijving,
                Zaaktype = ozZaaktype.Url,
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


        private static DateTime ConvertNamedTimezoneToDateTime(string dateTimeWithNamedTimeZone)
        {
            var zoneIndex = dateTimeWithNamedTimeZone.IndexOf('[');
            var dateTime = zoneIndex >= 0 ? dateTimeWithNamedTimeZone.Substring(0, zoneIndex) : dateTimeWithNamedTimeZone;

            return DateTime.Parse(dateTime);
        }

        private static string Truncate(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input.Length > maxLength ? input.Substring(0, maxLength).TrimEnd() : input;
        }
    }

}

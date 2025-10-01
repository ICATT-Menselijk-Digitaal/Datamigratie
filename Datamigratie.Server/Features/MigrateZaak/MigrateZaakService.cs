using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaak
{

    public interface IMigrateZaakService
    {
        public Task<OzZaak> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId);
    }

    public class MigrateZaakService(IOpenZaakApiClient openZaakApiClient, IConfiguration configuration) : IMigrateZaakService
    {
        public async Task<OzZaak> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId)
        {
            CheckIfZaakAlreadyExists();

            var createZaakRequest = CreateOzZaakCreationRequest(detZaak, ozZaaktypeId);
            
            var createdZaak = await openZaakApiClient.CreateZaak(createZaakRequest);

            return createdZaak;
        }

        private static void CheckIfZaakAlreadyExists()
        {
            // TODO -> story DATA-48
        }

        private CreateOzZaakRequest CreateOzZaakCreationRequest(DetZaak detZaak, Guid ozZaaktypeId)
        {
            // First apply data transformation to follow OpenZaak constraints
            var openZaakBaseUrl = configuration.GetValue<string>("OpenZaakApi:BaseUrl");
            var url = $"{openZaakBaseUrl}/catalogi/api/v1/zaaktypen/{ozZaaktypeId}";

            var registratieDatum = detZaak.CreatieDatumTijd.ToString("yyyy-MM-dd");

            var startDatum = detZaak.Startdatum.ToString("yyyy-MM-dd");

            // Now create the request

            var createRequest = new CreateOzZaakRequest
            {
                Identificatie = detZaak.FunctioneleIdentificatie,
                Bronorganisatie = "999990639", // moet een valide rsin zijn
                Omschrijving = detZaak.Omschrijving,
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

    }

}

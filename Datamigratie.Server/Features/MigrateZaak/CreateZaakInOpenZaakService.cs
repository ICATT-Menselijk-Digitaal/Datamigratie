//using Datamigratie.Common.Services.Det;
//using Datamigratie.Common.Services.OpenZaak.Models;
//using Datamigratie.Server.Features.CreateZaakInOpenZaak.RetrieveDetZaak;

//namespace Datamigratie.Server.Features.CreateZaakInOpenZaak
//{
//    public interface ICreateZaakInOpenZaakService
//    {
//        Task<CreateZaakResult> CreateZaakInOpenZaak(string zaaknummer);
//    }

//    public class CreateZaakInOpenZaakService : ICreateZaakInOpenZaakService
//    {
//        private readonly IRetrieveDetZaakService _retrieveDetZaakService;
//        private readonly IOpenZaakApiClient _openZaakApiClient;
//        private readonly ILogger<CreateZaakInOpenZaakService> _logger;

//        public CreateZaakInOpenZaakService(
//            IRetrieveDetZaakService retrieveDetZaakService,
//            IOpenZaakApiClient openZaakApiClient,
//            ILogger<CreateZaakInOpenZaakService> logger)
//        {
//            _retrieveDetZaakService = retrieveDetZaakService;
//            _openZaakApiClient = openZaakApiClient;
//            _logger = logger;
//        }

//        public async Task<CreateZaakResult> CreateZaakInOpenZaak(string zaaknummer)
//        {
//            try
//            {
//                var detZaak = await _retrieveDetZaakService.GetZaakByZaaknummer(zaaknummer);
//                if (detZaak == null)
//                {
//                    return CreateZaakResult.Failed($"Zaak {zaaknummer} was niet gevonden in DET");
//                }

//                var createRequest = new CreateOzZaakRequest
//                {
//                    Identificatie = detZaak.ExterneIdentificatie,
//                    Bronorganisatie = "123456789",
//                    Omschrijving = detZaak.Omschrijving,
//                    Zaaktype = "https://dummy-zaaktype-url.com",
//                    VerantwoordelijkeOrganisatie = "123456789",
//                    Startdatum = detZaak.Startdatum.ToString("yyyy-MM-dd"),
//                    Registratiedatum = DateTime.Today.ToString("yyyy-MM-dd"),
//                    Vertrouwelijkheidaanduiding = "openbaar",
//                    Betalingsindicatie = "nvt",
//                    Archiefstatus = "nog_te_archiveren"
//                };

//                var createdZaak = await _openZaakApiClient.CreateZaak(createRequest);

//                return CreateZaakResult.Success(createdZaak);
//            }
//            catch (HttpRequestException ex) when (ex.Message.Contains("DET"))
//            {
//                _logger.LogError(ex, "DET was not accessible for zaak {Zaaknummer}", zaaknummer);
//                return CreateZaakResult.Failed("DET was niet bereikbaar");
//            }
//            catch (HttpRequestException ex) when (ex.Message.Contains("Validation failed"))
//            {
//                _logger.LogError(ex, "OpenZaak validation failed for zaak {Zaaknummer}", zaaknummer);
//                return CreateZaakResult.Failed($"Zaak van Zaaktype kon niet worden aangemaakt in OZ: {ex.Message}");
//            }
//            catch (HttpRequestException ex)
//            {
//                _logger.LogError(ex, "OpenZaak was not accessible for zaak {Zaaknummer}", zaaknummer);
//                return CreateZaakResult.Failed("OZ was niet bereikbaar");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error creating zaak {Zaaknummer} in OpenZaak", zaaknummer);
//                return CreateZaakResult.Failed("Er is een onverwachte fout opgetreden");
//            }
//        }
//    }

   
//}

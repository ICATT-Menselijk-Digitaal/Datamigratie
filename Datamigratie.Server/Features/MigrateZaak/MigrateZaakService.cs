﻿using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.MigrateZaak
{
    public interface IMigrateZaakService
    {
        public Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId);
    }

    public class MigrateZaakService(IOpenZaakApiClient openZaakApiClient, IOptions<OpenZaakApiOptions> options) : IMigrateZaakService
    {
        private readonly OpenZaakApiOptions _openZaakApiOptions = options.Value;
        private readonly IOpenZaakApiClient _openZaakApiClient = openZaakApiClient;

        public async Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId)
        {

            try
            {
                CheckIfZaakAlreadyExists();

                var createZaakRequest = CreateOzZaakCreationRequest(detZaak, ozZaaktypeId);

                var createdZaak = await _openZaakApiClient.CreateZaak(createZaakRequest);

                return MigrateZaakResult.Success(createdZaak.Identificatie, "De zaak is aangemaakt in het doelsysteem");
            }
            catch (Exception ex)
            {
                var status = (ex is HttpRequestException httpRequestException && httpRequestException.StatusCode.HasValue)
                    ? (int)httpRequestException.StatusCode
                    : StatusCodes.Status500InternalServerError;

                return MigrateZaakResult.Failed(detZaak.FunctioneleIdentificatie, "De zaak kon niet worden aangemaakt in het doelsysteem.", ex.Message, status);
            }

         
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


    public class MigrateZaakResult
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public string Zaaknummer { get; private set; }
        public string? Details { get; private set; }
        public int? Statuscode { get; private set; }

        private MigrateZaakResult(bool isSuccess, string zaaknummer, string? message = null, string? details = null, int? statuscode = null)
        {
            IsSuccess = isSuccess;
            Zaaknummer = zaaknummer;
            Message = message;
            Details = details;
            Statuscode = statuscode;
        }
        public static MigrateZaakResult Success(string zaaknummer, string messsage) => new(true, zaaknummer, messsage);
        public static MigrateZaakResult Failed(string zaaknummer, string messsage, string details, int? statuscode) => new(false, zaaknummer, messsage, details, statuscode);
    }


}

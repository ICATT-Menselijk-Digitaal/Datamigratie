using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaak.Pdf;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Datamigratie.Server.Features.MigrateZaak
{
    public interface IMigrateZaakService
    {
        public Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId, CancellationToken token = default);
    }

    public class MigrateZaakService(
        IOpenZaakApiClient openZaakApiClient, 
        IDetApiClient detClient, 
        IOptions<OpenZaakApiOptions> options,
        IZaakgegevensPdfGenerator pdfGenerator) : IMigrateZaakService
    {
        private readonly OpenZaakApiOptions _openZaakApiOptions = options.Value;
        private readonly IOpenZaakApiClient _openZaakApiClient = openZaakApiClient;

        public async Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, Guid ozZaaktypeId, CancellationToken token = default)
        {

            try
            {
                var createZaakRequest = CreateOzZaakCreationRequest(detZaak, ozZaaktypeId);

                var createdZaak = await _openZaakApiClient.CreateZaak(createZaakRequest);
                var informatieObjectTypen = await _openZaakApiClient.GetInformatieobjecttypenUrlsForZaaktype(createdZaak.Zaaktype);
                var firstInformatieObjectType = informatieObjectTypen.First();

                await UploadZaakgegevensPdfAsync(detZaak, createdZaak, firstInformatieObjectType, token);

                // Migrate all documents with their versions
                await MigrateDocumentsAsync(detZaak, createdZaak, firstInformatieObjectType, token);

                return MigrateZaakResult.Success(createdZaak.Identificatie, "De zaak is aangemaakt in het doelsysteem");
            }
            catch (HttpRequestException httpEx)
            {
                return MigrateZaakResult.Failed(
                    detZaak.FunctioneleIdentificatie,
                    "De zaak kon niet worden aangemaakt in het doelsysteem.",
                    httpEx.Message,
                    (int?)httpEx.StatusCode ?? StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                var statusCode = ex.InnerException is HttpRequestException innerHttpEx
                    ? (int?)innerHttpEx.StatusCode ?? StatusCodes.Status500InternalServerError
                    : StatusCodes.Status500InternalServerError;

                return MigrateZaakResult.Failed(
                    detZaak.FunctioneleIdentificatie,
                    "De zaak kon niet worden aangemaakt in het doelsysteem.",
                    ex.Message,
                    statusCode);
            }
        }

        private static OzDocument MapToOzDocument(DetDocument item, DetDocumentVersie versie, Uri informatieObjectType)
        {
            // Apply data transformations
            const int MaxTitelLength = 200; // 197 + "..."
            var titel = TruncateWithDots(item.Titel, MaxTitelLength);

            const int MaxBeschijvingLength = 1000; // 997 + "..."
            var beschrijving = TruncateWithDots(item.Beschrijving, MaxBeschijvingLength);

            const int MaxIdentificatieLength = 40;

            // If kenmerk is longer than 40, fail the migration
            if (item.Kenmerk?.Length > MaxIdentificatieLength)
            {
                throw new InvalidDataException($"Document '{item.Titel}' migration failed: The 'kenmerk' field length ({item.Kenmerk.Length}) exceeds the maximum allowed length of {MaxIdentificatieLength} characters.");
            }

            return new OzDocument
            {
                Bestandsnaam = versie.Bestandsnaam,
                Bronorganisatie = "999990639", // moet een valide rsin zijn,
                Formaat = versie.Mimetype,
                Identificatie = item.Kenmerk,
                Informatieobjecttype = informatieObjectType,
                Taal = "dut",
                Titel = titel,
                Vertrouwelijkheidaanduiding = VertrouwelijkheidsAanduiding.openbaar,
                Bestandsomvang = versie.Documentgrootte,
                Auteur = "onbekend",
                Beschrijving = beschrijving,
                Creatiedatum = versie.Creatiedatum,
                Status = DocumentStatus.in_bewerking,
                Trefwoorden = [],
                Verschijningsvorm = item?.DocumentVorm?.Naam,
                Link = ""
            };
        }
        private async Task CreateAndLinkDocumentAsync(
            OzDocument ozDocument, 
            OzZaak zaak, 
            Func<OzDocument, CancellationToken, Task> uploadContentAction,
            CancellationToken token)
        {
            var savedDocument = await _openZaakApiClient.CreateDocument(ozDocument);
            
            await uploadContentAction(savedDocument, token);
            
            await _openZaakApiClient.UnlockDocument(savedDocument.Id, savedDocument.Lock, token);
            await _openZaakApiClient.KoppelDocument(zaak, savedDocument, token);
        }

        private async Task UploadZaakgegevensPdfAsync(DetZaak detZaak, OzZaak createdZaak, Uri informatieObjectType, CancellationToken token)
        {
            var pdfBytes = pdfGenerator.GenerateZaakgegevensPdf(detZaak);
            var fileName = $"zaakgegevens_{detZaak.FunctioneleIdentificatie}.pdf";

            var ozDocument = new OzDocument
            {
                Bestandsnaam = fileName,
                Bronorganisatie = "999990639",
                Formaat = "application/pdf",
                Identificatie = $"zaakgegevens-{detZaak.FunctioneleIdentificatie}",
                Informatieobjecttype = informatieObjectType,
                Taal = "dut",
                Titel = $"Zaakgegevens {detZaak.FunctioneleIdentificatie}",
                Vertrouwelijkheidaanduiding = VertrouwelijkheidsAanduiding.openbaar,
                Bestandsomvang = pdfBytes.Length,
                Auteur = "Datamigratie",
                Beschrijving = "Automatisch gegenereerd document met basisgegevens van de zaak uit het bronsysteem",
                Creatiedatum = DateOnly.FromDateTime(DateTime.Now),
                Status = DocumentStatus.in_bewerking,
                Trefwoorden = [],
                Verschijningsvorm = "verschijningsvorm",
                Link = ""
            };

            await CreateAndLinkDocumentAsync(
                ozDocument,
                createdZaak,
                async (savedDoc, ct) =>
                {
                    using var pdfStream = new MemoryStream(pdfBytes);
                    await _openZaakApiClient.UploadBestand(savedDoc, pdfStream, ct);
                },
                token);
        }

        /// <summary>
        /// Migrates all documents with their versions in the correct order.
        /// First version is created, next versions update the same document (OpenZaak auto-increments version).
        /// </summary>
        private async Task MigrateDocumentsAsync(DetZaak detZaak, OzZaak createdZaak, Uri informatieObjectType, CancellationToken token)
        {
            foreach (var document in detZaak?.Documenten ?? [])
            {
                var sortedVersions = document.DocumentVersies.OrderBy(v => v.Versienummer).ToList();
                
                OzDocument? mainDocument = null;
                
                for (var i = 0; i < sortedVersions.Count; i++)
                {
                    var detVersie = sortedVersions[i];
                    var isFirstVersion = i == 0;
                    
                    try
                    {
                        var ozDocument = MapToOzDocument(document, detVersie, informatieObjectType);
                        
                        if (isFirstVersion)
                        {
                            // create new document and link to zaak
                            OzDocument? capturedDocument = null;

                            await CreateAndLinkDocumentAsync(
                                ozDocument,
                                createdZaak,
                                async (savedDoc, ct) =>
                                {
                                    capturedDocument = savedDoc;
                                    await detClient.GetDocumentInhoudAsync(
                                        detVersie.DocumentInhoudID,
                                        async (stream, streamCt) => await _openZaakApiClient.UploadBestand(savedDoc, stream, streamCt),
                                        ct);
                                },
                                token);
                                

                            mainDocument = capturedDocument;
                        }
                        else
                        {
                            // other versions: update existing document
                            if (mainDocument?.Id == null) {
                                throw new InvalidOperationException("First document version must be created before updating");
                            }

                            // lock the document to get a lock token
                            var lockToken = await _openZaakApiClient.LockDocument(mainDocument.Id, token);
                            
                            // set lock token
                            ozDocument.Lock = lockToken;

                            // update document to create new version
                            var updatedDocument = await _openZaakApiClient.UpdateDocument(mainDocument.Id, ozDocument);

                            // after an update the document contains outdated bestandsdelen information. 
                            // we need to GET a document again in order to get the latest bestandsdelen
                            var refreshedDocument = await _openZaakApiClient.GetDocument(mainDocument.Id);

                            if (refreshedDocument == null)
                            {
                                throw new InvalidDataException($"We cannot find the document with id {mainDocument.Id} that was updated.");
                            }

                            // set lock token again
                            refreshedDocument.Lock = lockToken;
                            
                            await detClient.GetDocumentInhoudAsync(
                                detVersie.DocumentInhoudID,
                                async (stream, ct) => await openZaakApiClient.UploadBestand(refreshedDocument, stream, ct),
                                token);
                            
                            await _openZaakApiClient.UnlockDocument(mainDocument.Id, lockToken, token);
                        }
                    }
                    catch (Exception ex)
                    {
                        var httpStatusInfo = ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue
                            ? $" | HTTP {(int)httpEx.StatusCode}: {httpEx.Message}"
                            : ex is HttpRequestException httpExOuter && httpExOuter.StatusCode.HasValue
                            ? $" | HTTP {(int)httpExOuter.StatusCode}: {httpExOuter.Message}"
                            : "";

                        throw new Exception(
                            $"Migratie onderbroken: versie {detVersie.Versienummer} van document '{document.Titel}' (bestand: {detVersie.Bestandsnaam}) kon niet worden gemigreerd{httpStatusInfo}",
                            ex);
                    }
                }
            }
        }

        private CreateOzZaakRequest CreateOzZaakCreationRequest(DetZaak detZaak, Guid ozZaaktypeId)
        {
            // First apply data transformation to follow OpenZaak constraints
            var openZaakBaseUrl = _openZaakApiOptions.BaseUrl;
            var url = new Uri($"{openZaakBaseUrl}catalogi/api/v1/zaaktypen/{ozZaaktypeId}");

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
        [return: NotNullIfNotNull(nameof(input))]
        private static string? TruncateWithDots(string? input, int maxLength)
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

            var truncatedInput = input[..(maxLength - dots.Length)].TrimEnd();

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

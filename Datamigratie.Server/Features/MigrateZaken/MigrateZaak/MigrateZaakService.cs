using System.Diagnostics.CodeAnalysis;
using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.Migrate.MigrateZaak.Models;
using Datamigratie.Server.Features.Migrate.MigrateZaak.Pdf;
using Microsoft.Extensions.Options;

namespace Datamigratie.Server.Features.Migrate.MigrateZaak
{
    public interface IMigrateZaakService
    {
        public Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, MigrateZaakMappingModel mapping, CancellationToken token = default);
    }

    public class MigrateZaakService(
        IOpenZaakApiClient openZaakApiClient,
        IDetApiClient detClient,
        IOptions<OpenZaakApiOptions> options,
        IZaakgegevensPdfGenerator pdfGenerator) : IMigrateZaakService
    {
        private readonly OpenZaakApiOptions _openZaakApiOptions = options.Value;
        private readonly IOpenZaakApiClient _openZaakApiClient = openZaakApiClient;

        public async Task<MigrateZaakResult> MigrateZaak(DetZaak detZaak, MigrateZaakMappingModel mapping, CancellationToken token = default)
        {
            try
            {
                var createZaakRequest = CreateOzZaakCreationRequest(detZaak, mapping.OpenZaaktypeId, mapping.Rsin);

                var createdZaak = await _openZaakApiClient.CreateZaak(createZaakRequest);

                var informatieObjectTypen = await _openZaakApiClient.GetInformatieobjecttypenUrlsForZaaktype(createdZaak.Zaaktype);
                var firstInformatieObjectType = informatieObjectTypen.First();

                // Create resultaat for the zaak based on resultaat mapping (must be run before status)
                await MigrateResultaatAsync(detZaak, createdZaak, mapping, token);

                // Create status for the zaak based on status mapping
                await MigrateStatusAsync(detZaak, createdZaak, mapping, token);

                await UploadZaakgegevensPdfAsync(detZaak, createdZaak, firstInformatieObjectType, mapping.Rsin, token);

                // Migrate all documents with their versions
                await MigrateDocumentsAsync(detZaak, createdZaak, firstInformatieObjectType, mapping.Rsin, mapping.DocumentstatusMappings, mapping.DocumentPropertyMappings, token);

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

        private static OzDocument MapToOzDocument(DetDocument item, DetDocumentVersie versie, Uri informatieObjectType, string rsin, Dictionary<string, string> documentstatusMappings, Dictionary<string, Dictionary<string, string>> documentPropertyMappings)
        {
            // Apply data transformations
            const int MaxTitelLength = 200; // 197 + "..."
            var titel = TruncateWithDots(item.Titel, MaxTitelLength);

            const int MaxBeschijvingLength = 1000; // 997 + "..."
            var beschrijving = TruncateWithDots(item.Beschrijving, MaxBeschijvingLength);

            beschrijving ??= "";

            var verschijningsvorm = item?.DocumentVorm?.Naam ?? "";

            const int MaxIdentificatieLength = 40;

            // If kenmerk is longer than 40, fail the migration
            if (item.Kenmerk?.Length > MaxIdentificatieLength)
            {
                throw new InvalidDataException($"Document '{item.Titel}' migration failed: The 'kenmerk' field length ({item.Kenmerk.Length}) exceeds the maximum allowed length of {MaxIdentificatieLength} characters.");
            }

            // Map the document status from DET to OpenZaak
            var ozDocumentStatus = GetOzDocumentStatus(item, documentstatusMappings);

            var vertrouwelijkheidaanduiding = MapPublicatieNiveau(item.Publicatieniveau, documentPropertyMappings, item.Titel);

            var informatieobjecttype = MapDocumenttype(item.Documenttype?.Naam, informatieObjectType, documentPropertyMappings, item.Titel);

            var taal = item.Taal?.FunctioneelId.ToLower() ?? "dut";
            var auteur = versie.Auteur ?? "Auteur_onbekend";

            return new OzDocument
            {
                Bestandsnaam = versie.Bestandsnaam,
                Bronorganisatie = rsin,
                Formaat = versie.Mimetype,
                Identificatie = item.Kenmerk,
                Informatieobjecttype = informatieobjecttype,
                Taal = taal,
                Titel = titel,
                Vertrouwelijkheidaanduiding = vertrouwelijkheidaanduiding,
                Bestandsomvang = versie.Documentgrootte,
                Auteur = auteur,
                Beschrijving = beschrijving,
                Creatiedatum = versie.Creatiedatum,
                Status = ozDocumentStatus,
                Verschijningsvorm = verschijningsvorm,
                Link = "",
                Trefwoorden = []
            };
        }

        private static VertrouwelijkheidsAanduiding MapPublicatieNiveau(string? publicatieNiveau, Dictionary<string, Dictionary<string, string>> documentPropertyMappings, string documentTitel)
        {
            if (string.IsNullOrWhiteSpace(publicatieNiveau))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Publicatieniveau is required but was not provided.");
            }

            if (!documentPropertyMappings.TryGetValue("publicatieniveau", out var publicatieNiveauMappings))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: No publicatieniveau mappings configured.");
            }

            if (!publicatieNiveauMappings.TryGetValue(publicatieNiveau, out var mappedValue))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Publicatieniveau '{publicatieNiveau}' has not been mapped to an OpenZaak vertrouwelijkheidaanduiding.");
            }

            if (!Enum.TryParse<VertrouwelijkheidsAanduiding>(mappedValue, true, out var vertrouwelijkheid))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Mapped vertrouwelijkheidaanduiding '{mappedValue}' is not a valid value.");
            }

            return vertrouwelijkheid;
        }

        private static Uri MapDocumenttype(string? documenttypeNaam, Uri defaultInformatieobjecttype, Dictionary<string, Dictionary<string, string>> documentPropertyMappings, string documentTitel)
        {
            if (string.IsNullOrWhiteSpace(documenttypeNaam))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Documenttype is required but was not provided.");
            }

            if (!documentPropertyMappings.TryGetValue("documenttype", out var documenttypeMappings))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: No documenttype mappings configured.");
            }

            if (!documenttypeMappings.TryGetValue(documenttypeNaam, out var mappedValue))
            {
                throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Documenttype '{documenttypeNaam}' has not been mapped to an OpenZaak informatieobjecttype.");
            }

            if (Guid.TryParse(mappedValue, out var guid))
            {
                var baseUri = defaultInformatieobjecttype.GetLeftPart(UriPartial.Path);
                var uriWithoutId = baseUri.Substring(0, baseUri.LastIndexOf('/') + 1);
                return new Uri($"{uriWithoutId}{guid}");
            }

            return Uri.TryCreate(mappedValue, UriKind.Absolute, out var uri)
                ? uri
                : throw new InvalidOperationException($"Document '{documentTitel}' migration failed: Mapped informatieobjecttype value '{mappedValue}' is neither a valid GUID nor a valid URI.");
        }

        private static DocumentStatus GetOzDocumentStatus(DetDocument document, Dictionary<string, string> documentstatusMappings)
        {
            // Look up the mapping for this document status
            if (!documentstatusMappings.TryGetValue(document.Documentstatus.Naam, out var ozStatusString))
            {
                throw new InvalidOperationException($"Document '{document.Titel}' migration failed: No mapping found for DET document status '{document.Documentstatus.Naam}'.");
            }

            // Parse the string to the enum
            if (!Enum.TryParse<DocumentStatus>(ozStatusString, out var ozStatus))
            {
                throw new InvalidOperationException($"Document '{document.Titel}' migration failed: Invalid OpenZaak document status '{ozStatusString}' configured for DET status '{document.Documentstatus.Naam}'.");
            }

            return ozStatus;
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

        private async Task UploadZaakgegevensPdfAsync(DetZaak detZaak, OzZaak createdZaak, Uri informatieObjectType, string rsin, CancellationToken token)
        {
            var pdfBytes = pdfGenerator.GenerateZaakgegevensPdf(detZaak);
            var fileName = $"zaakgegevens_{detZaak.FunctioneleIdentificatie}.pdf";

            var ozDocument = new OzDocument
            {
                Bestandsnaam = fileName,
                Bronorganisatie = rsin,
                Formaat = "application/pdf",
                Identificatie = $"zaakgegevens-{detZaak.FunctioneleIdentificatie}",
                Informatieobjecttype = informatieObjectType,
                Taal = "dut",
                Titel = $"e-Suite zaakgegevens {detZaak.FunctioneleIdentificatie}",
                Vertrouwelijkheidaanduiding = VertrouwelijkheidsAanduiding.openbaar,
                Bestandsomvang = pdfBytes.Length,
                Auteur = "Automatisch gegenereerd bij migratie vanuit e-Suite",
                Beschrijving = "Automatisch gegenereerd document met basisgegevens van de zaak uit het bronsysteem",
                Creatiedatum = DateOnly.FromDateTime(DateTime.Now),
                Status = DocumentStatus.definitief,
                Link = "",
                Verschijningsvorm = "",
                Trefwoorden = [],
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

        private async Task MigrateResultaatAsync(DetZaak detZaak, OzZaak createdZaak, MigrateZaakMappingModel mapping, CancellationToken token)
        {
            if (mapping.ResultaattypeUri == null)
            {
                return;
            }

            // Create the resultaat in OpenZaak
            var createResultaatRequest = new CreateOzResultaatRequest
            {
                Zaak = createdZaak.Url,
                Resultaattype = mapping.ResultaattypeUri,
                Toelichting = "Resultaat gemigreerd vanuit e-Suite"
            };

            await _openZaakApiClient.CreateResultaat(createResultaatRequest);
        }

        private async Task MigrateStatusAsync(DetZaak detZaak, OzZaak createdZaak, MigrateZaakMappingModel mapping, CancellationToken token)
        {
            if (mapping.StatustypeUri == null)
            {
                return;
            }

            if (!detZaak.Einddatum.HasValue)
            {
                throw new InvalidOperationException(
                    $"Zaak {detZaak.FunctioneleIdentificatie} has no einddatum. Cannot determine datumStatusGezet.");
            }

            var datumStatusGezet = detZaak.Einddatum.Value.ToDateTime(TimeOnly.MinValue);
            var createStatusRequest = new CreateOzStatusRequest
            {
                Zaak = createdZaak.Url,
                Statustype = mapping.StatustypeUri,
                DatumStatusGezet = datumStatusGezet,
                Statustoelichting = $"Status gemigreerd vanuit e-Suite"
            };

            await _openZaakApiClient.CreateStatus(createStatusRequest);
        }

        /// <summary>
        /// Migrates all documents with their versions in the correct order.
        /// First version is created, next versions update the same document (OpenZaak auto-increments version).
        /// </summary>
        private async Task MigrateDocumentsAsync(DetZaak detZaak, OzZaak createdZaak, Uri informatieObjectType, string rsin, Dictionary<string, string> documentstatusMappings, Dictionary<string, Dictionary<string, string>> documentPropertyMappings, CancellationToken token)
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
                        var ozDocument = MapToOzDocument(document, detVersie, informatieObjectType, rsin, documentstatusMappings, documentPropertyMappings);
                        
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
                            if (mainDocument?.Id == null)
                            {
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
                                async (stream, ct) => await _openZaakApiClient.UploadBestand(refreshedDocument, stream, ct),
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

        private CreateOzZaakRequest CreateOzZaakCreationRequest(DetZaak detZaak, Guid ozZaaktypeId, string rsin)
        {
            // First apply data transformation to follow OpenZaak constraints
            var openZaakBaseUrl = _openZaakApiOptions.BaseUrl;
            var url = new Uri($"{openZaakBaseUrl}catalogi/api/v1/zaaktypen/{ozZaaktypeId}");

            var registratieDatum = detZaak.CreatieDatumTijd.ToString("yyyy-MM-dd");

            var startDatum = detZaak.Startdatum.ToString("yyyy-MM-dd");

            const int MaxOmschrijvingLength = 80;
            var omschrijving = TruncateWithDots(detZaak.Omschrijving, MaxOmschrijvingLength);

            const int MaxZaaknummerLength = 40;

            if (detZaak.FunctioneleIdentificatie.Length > MaxZaaknummerLength)
            {
                throw new InvalidDataException($"Zaak '{detZaak.FunctioneleIdentificatie}' migration failed: The 'zaaknummer' field length ({detZaak.FunctioneleIdentificatie.Length}) exceeds the maximum allowed length of {MaxZaaknummerLength} characters.");
            }

            // Now create the request
            var createRequest = new CreateOzZaakRequest
            {
                Identificatie = detZaak.FunctioneleIdentificatie,
                Bronorganisatie = rsin, // moet een valide rsin zijn
                Omschrijving = omschrijving,
                Zaaktype = url,
                VerantwoordelijkeOrganisatie = rsin,  // moet een valide rsin zijn
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

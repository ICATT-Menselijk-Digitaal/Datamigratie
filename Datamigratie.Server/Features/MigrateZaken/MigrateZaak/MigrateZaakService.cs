using System.Diagnostics;
using System.Diagnostics.Metrics;
using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Pdf;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Plan;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak
{
    public interface IMigrateZaakService
    {
        public Task<MigrateZaakResult> MigrateZaak(string zaaknummer, MigrateZaakMappingModel mapping, CancellationToken token = default);
    }

    public class MigrateZaakService(
        IOpenZaakApiClient openZaakApiClient,
        IDetApiClient detClient,
        IZaakgegevensPdfGenerator pdfGenerator,
        ILogger<MigrateZaakService> logger) : IMigrateZaakService
    {
        private static readonly ActivitySource ActivitySource = new("Datamigratie.Server");
        private static readonly Meter Meter = new("Datamigratie.Server");

        // End-to-end duration of a single zaak migration, tagged result=succeeded|failed
        private static readonly Histogram<double> ZaakDurationHistogram =
            Meter.CreateHistogram<double>("migration.zaak.duration", "ms", "End-to-end duration of migrating a single zaak");

        // Number of documents and total versions per zaak — context for duration outliers
        private static readonly Histogram<int> ZaakDocumentCountHistogram =
            Meter.CreateHistogram<int>("migration.zaak.document.count", "{document}", "Number of documents per zaak");

        private static readonly Histogram<int> ZaakDocumentVersionCountHistogram =
            Meter.CreateHistogram<int>("migration.zaak.document.version.count", "{version}", "Total document versions per zaak");

        private readonly IOpenZaakApiClient _openZaakApiClient = openZaakApiClient;

        public async Task<MigrateZaakResult> MigrateZaak(string zaaknummer, MigrateZaakMappingModel mapping, CancellationToken token = default)
        {
            using var activity = ActivitySource.StartActivity("MigrateZaak", ActivityKind.Internal);
            activity?.SetTag("zaak.identificatie", zaaknummer);
            var sw = Stopwatch.StartNew();

            DetZaak detZaak;
            try
            {
                detZaak = await FetchZaakFromDetAsync(zaaknummer);
            }
            catch (HttpRequestException httpEx)
            {
                sw.Stop();
                ZaakDurationHistogram.Record(sw.Elapsed.TotalMilliseconds, new TagList { { "result", "failed" } });
                activity?.SetTag("zaak.result", "failed");
                activity?.SetStatus(ActivityStatusCode.Error, httpEx.Message);

                var statusCode = (int?)httpEx.StatusCode ?? StatusCodes.Status500InternalServerError;
                return MigrateZaakResult.Failed(
                    zaaknummer,
                    "De zaak kon niet opgehaald worden uit het bronsysteem.",
                    $"HTTP {statusCode}: {httpEx.Message}",
                    statusCode);
            }

            try
            {
                // Phase A: Pure plan
                var plan = ZaakMigrationPlanBuilder.Build(detZaak, mapping);

                // Phase B: Execute

                // Check if zaak already exists in OpenZaak and delete it to allow re-run
                var existingZaak = await _openZaakApiClient.GetZaakByIdentificatie(detZaak.FunctioneleIdentificatie);
                if (existingZaak != null)
                {
                    await DeleteExistingZaakAndRelatedObjectsAsync(existingZaak, token);
                }

                OzZaak createdZaak;
                using (ActivitySource.StartActivity("CreateZaak"))
                {
                    createdZaak = await _openZaakApiClient.CreateZaak(plan.ZaakRequest);
                }

                // Create resultaat for the zaak based on resultaat mapping (must be run before status)
                await ExecuteResultaatPlanAsync(plan.Resultaat, createdZaak, token);

                // Create status for the zaak based on status mapping
                await ExecuteStatusPlanAsync(plan.Status, createdZaak, token);

                // Generate and upload the zaakgegevens PDF
                await ExecutePdfPlanAsync(plan.PdfDocument, detZaak, createdZaak, token);

                // Migrate all documents with their versions
                await ExecuteDocumentPlansAsync(plan.Documents, createdZaak, token);

                // Migrate all besluiten for the zaak
                await ExecuteBesluitPlansAsync(plan.Besluiten, createdZaak, token);

                // Migrate rollen (e.g. Behandelaar) to OpenZaak
                await ExecuteRolPlansAsync(plan.Rollen, createdZaak, token);

                try
                {
                    await detClient.SetZaakGemigreerd(detZaak.FunctioneleIdentificatie, true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Zaak {detZaak.FunctioneleIdentificatie} gemigreerd to OpenZaak with ID {createdZaak.Identificatie}, but failed to update migration status in DET. Original exception message: {ex.Message}",
                        ex);
                }

                sw.Stop();

                var documentCount = detZaak.Documenten?.Count ?? 0;
                var versionCount = detZaak.Documenten?.Sum(d => d.DocumentVersies.Count) ?? 0;
                var besluitCount = detZaak.Besluiten?.Count ?? 0;

                ZaakDurationHistogram.Record(sw.Elapsed.TotalMilliseconds, new TagList { { "result", "succeeded" } });
                ZaakDocumentCountHistogram.Record(documentCount, new TagList { { "has_documents", documentCount > 0 } });
                ZaakDocumentVersionCountHistogram.Record(versionCount, new TagList { { "has_documents", documentCount > 0 } });

                activity?.SetTag("zaak.result", "succeeded");
                activity?.SetTag("zaak.duration_ms", sw.Elapsed.TotalMilliseconds);
                activity?.SetTag("zaak.document.count", documentCount);
                activity?.SetTag("zaak.document.version.count", versionCount);
                activity?.SetTag("zaak.besluit.count", besluitCount);

                return MigrateZaakResult.Success(createdZaak.Identificatie, "De zaak is aangemaakt in het doelsysteem");
            }
            catch (HttpRequestException httpEx)
            {
                sw.Stop();
                ZaakDurationHistogram.Record(sw.Elapsed.TotalMilliseconds, new TagList { { "result", "failed" } });
                activity?.SetTag("zaak.result", "failed");
                activity?.SetTag("zaak.duration_ms", sw.Elapsed.TotalMilliseconds);
                activity?.SetStatus(ActivityStatusCode.Error, httpEx.Message);

                return MigrateZaakResult.Failed(
                    zaaknummer,
                    "De zaak kon niet worden aangemaakt in het doelsysteem.",
                    httpEx.Message,
                    (int?)httpEx.StatusCode ?? StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                sw.Stop();
                ZaakDurationHistogram.Record(sw.Elapsed.TotalMilliseconds, new TagList { { "result", "failed" } });
                activity?.SetTag("zaak.result", "failed");
                activity?.SetTag("zaak.duration_ms", sw.Elapsed.TotalMilliseconds);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                var statusCode = ex.InnerException is HttpRequestException innerHttpEx
                    ? (int?)innerHttpEx.StatusCode ?? StatusCodes.Status500InternalServerError
                    : StatusCodes.Status500InternalServerError;

                return MigrateZaakResult.Failed(
                    zaaknummer,
                    "De zaak kon niet worden aangemaakt in het doelsysteem.",
                    ex.Message,
                    statusCode);
            }
        }

        private async Task CreateAndLinkDocumentAsync(
            OzDocument ozDocument,
            OzZaak zaak,
            Func<OzDocument, CancellationToken, Task> uploadContentAction,
            CancellationToken token)
        {
            var savedDocument = await _openZaakApiClient.CreateDocument(ozDocument);

            try
            {
                await uploadContentAction(savedDocument, token);
            }
            catch
            {
                await TryUnlockDocumentIgnoringErrorsAsync(savedDocument.Id, savedDocument.Lock, token);
                throw;
            }
            await _openZaakApiClient.UnlockDocument(savedDocument.Id, savedDocument.Lock, token);

            await _openZaakApiClient.KoppelDocument(zaak, savedDocument, token);
        }

        private async Task UpdateDocumentVersionAsync(Guid documentId, OzDocument ozDocument, long documentInhoudId, CancellationToken token)
        {
            var lockToken = await _openZaakApiClient.LockDocument(documentId, token);

            try
            {
                ozDocument.Lock = lockToken;

                // update document to create new version
                await _openZaakApiClient.UpdateDocument(documentId, ozDocument);

                // after an update the document contains outdated bestandsdelen information.
                // we need to GET a document again in order to get the latest bestandsdelen
                var refreshedDocument = await _openZaakApiClient.GetDocument(documentId)
                    ?? throw new InvalidDataException($"We cannot find the document with id {documentId} that was updated.");

                refreshedDocument.Lock = lockToken;

                await detClient.GetDocumentInhoudAsync(
                    documentInhoudId,
                    async (stream, ct) => await _openZaakApiClient.UploadBestand(refreshedDocument, stream, ct),
                    token);
            }
            catch
            {
                await TryUnlockDocumentIgnoringErrorsAsync(documentId, lockToken, token);
                throw;
            }
            await _openZaakApiClient.UnlockDocument(documentId, lockToken, token);
        }

        private async Task TryUnlockDocumentIgnoringErrorsAsync(Guid documentId, string? lockToken, CancellationToken token)
        {
            try
            {
                await _openZaakApiClient.UnlockDocument(documentId, lockToken, token);
            }
            catch (Exception ex)
            {
                // Swallow unlock failures so the original exception propagates that triggered this unlock attempt
                logger.LogError(ex, "Failed to unlock document {DocumentId} after an error. The document may remain locked in OpenZaak.", documentId);
            }
        }

        private async Task ExecutePdfPlanAsync(OzDocument ozDocument, DetZaak detZaak, OzZaak createdZaak, CancellationToken token)
        {
            using var activity = ActivitySource.StartActivity("UploadZaakgegevensPdf");

            using var stream = new MemoryStream();
            using (ActivitySource.StartActivity("GenerateZaakgegevensPdf"))
            {
                pdfGenerator.GenerateZaakgegevensPdf(detZaak, stream);
            }

            // Create document with the actual PDF size
            ozDocument.Bestandsomvang = stream.Length;

            await CreateAndLinkDocumentAsync(
                ozDocument,
                createdZaak,
                async (savedDoc, ct) =>
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await _openZaakApiClient.UploadBestand(savedDoc, stream, ct);
                },
                token);
        }

        private async Task ExecuteResultaatPlanAsync(CreateOzResultaatRequest? plan, OzZaak createdZaak, CancellationToken token)
        {
            if (plan == null) return;

            using var activity = ActivitySource.StartActivity("MigrateResultaat");
            plan.Zaak = createdZaak.Url;

            await _openZaakApiClient.CreateResultaat(plan);
        }

        private async Task ExecuteStatusPlanAsync(CreateOzStatusRequest? plan, OzZaak createdZaak, CancellationToken token)
        {
            if (plan == null) return;

            using var activity = ActivitySource.StartActivity("MigrateStatus");
            plan.Zaak = createdZaak.Url;

            await _openZaakApiClient.CreateStatus(plan);
        }

        private async Task ExecuteRolPlansAsync(IReadOnlyCollection<OzCreateRolRequest> rollen, OzZaak createdZaak, CancellationToken token)
        {
            List<OzRoltype>? ozRoltypes = null;
            foreach (var rol in rollen)
            {
                rol.Zaak = createdZaak.Url;
                try
                {
                    await _openZaakApiClient.CreateRol(rol, token);
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("max-occurences", StringComparison.OrdinalIgnoreCase))
                {
                    ozRoltypes ??= await _openZaakApiClient.GetRoltypesForZaaktype(createdZaak.Url);
                    var rolTypeUrl = rol.Roltype.ToString();
                    var rolType = ozRoltypes.FirstOrDefault(rt => rt.Url == rolTypeUrl);
                    var omschrijving = rolType?.Omschrijving ?? "ONBEKEND";
                    var omschrijvingGeneriek = rolType?.OmschrijvingGeneriek ?? "ONBEKEND";
                    throw new InvalidOperationException(
                        $"De rol '{omschrijving}' kan niet worden aangemaakt omdat OpenZaak het maximale aantal exemplaren voor dit roltype '{omschrijvingGeneriek}' al heeft bereikt. " +
                        $"Pas de roltypemapping aan zodat het roltype '{omschrijvingGeneriek}' niet meer dan één keer wordt toegewezen aan dezelfde zaak.",
                        ex);
                }
            }
        }

        /// <summary>
        /// Migrates all besluiten for a zaak to OpenZaak.
        /// </summary>
        private async Task ExecuteBesluitPlansAsync(IReadOnlyList<CreateOzBesluitRequest> besluitPlans, OzZaak createdZaak, CancellationToken token)
        {
            using var activity = ActivitySource.StartActivity("MigrateBesluiten");

            if (besluitPlans.Count == 0) return;

            foreach (var ozBesluitRequest in besluitPlans)
            {
                try
                {
                    ozBesluitRequest.Zaak = createdZaak.Url;
                    await _openZaakApiClient.CreateBesluit(ozBesluitRequest);
                }
                catch (Exception ex)
                {
                    var httpStatusInfo = ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue
                        ? $" | HTTP {(int)httpEx.StatusCode}: {httpEx.Message}"
                        : ex is HttpRequestException httpExOuter && httpExOuter.StatusCode.HasValue
                        ? $" | HTTP {(int)httpExOuter.StatusCode}: {httpExOuter.Message}"
                        : $" | {ex.GetType().Name}: {ex.Message}";

                    throw new Exception(
                        $"Migratie onderbroken: besluit '{ozBesluitRequest.Identificatie}' kon niet worden gemigreerd{httpStatusInfo}",
                        ex);
                }
            }
        }

        /// <summary>
        /// Migrates all documents with their versions in the correct order.
        /// First version is created, next versions update the same document (OpenZaak auto-increments version).
        /// </summary>
        private async Task ExecuteDocumentPlansAsync(IReadOnlyList<DocumentMigrationPlan> documentPlans, OzZaak createdZaak, CancellationToken token)
        {
            using var activity = ActivitySource.StartActivity("MigrateDocuments");
            activity?.SetTag("zaak.document_count", documentPlans.Count);

            foreach (var documentPlan in documentPlans)
            {
                OzDocument? mainDocument = null;

                for (var i = 0; i < documentPlan.Versions.Count; i++)
                {
                    var versionPlan = documentPlan.Versions[i];
                    var isFirstVersion = i == 0;

                    try
                    {
                        if (isFirstVersion)
                        {
                            // create new document and link to zaak
                            OzDocument? capturedDocument = null;

                            await CreateAndLinkDocumentAsync(
                                versionPlan.Document,
                                createdZaak,
                                async (savedDoc, ct) =>
                                {
                                    capturedDocument = savedDoc;
                                    await detClient.GetDocumentInhoudAsync(
                                        versionPlan.DocumentInhoudId,
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

                            await UpdateDocumentVersionAsync(mainDocument.Id, versionPlan.Document, versionPlan.DocumentInhoudId, token);
                        }
                    }
                    catch (Exception ex)
                    {
                        var httpStatusInfo = ex.InnerException is HttpRequestException httpEx && httpEx.StatusCode.HasValue
                            ? $" | HTTP {(int)httpEx.StatusCode}: {httpEx.Message}"
                            : ex is HttpRequestException httpExOuter && httpExOuter.StatusCode.HasValue
                            ? $" | HTTP {(int)httpExOuter.StatusCode}: {httpExOuter.Message}"
                            : $" | {ex.GetType().Name}: {ex.Message}";

                        throw new Exception(
                            $"Migratie onderbroken: versie {i + 1} van document '{versionPlan.Document.Titel}' (bestand: {versionPlan.Document.Bestandsnaam}) kon niet worden gemigreerd{httpStatusInfo}",
                            ex);
                    }
                }
            }
        }

        private async Task<DetZaak> FetchZaakFromDetAsync(string zaaknummer)
        {
            logger.LogDebug("Fetching full details for zaak {Zaaknummer} from DET", zaaknummer);

            try
            {
                var zaak = await detClient.GetZaakByZaaknummer(zaaknummer);
                return zaak ?? throw new InvalidOperationException($"This zaaknumber '{zaaknummer}' is not found in the DET API");
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Failed to fetch zaak from DET API: {ex.Message}", ex, ex.StatusCode);
            }
        }

        /// <summary>
        /// Deletes an existing zaak and all its related objects from OpenZaak.
        ///
        /// Note: The DELETE zaak endpoint automatically cascades and deletes:
        /// - All statussen (statuses)
        /// - The resultaat (result)
        /// - All zaakinformatieobjecten (document links)
        /// - All rollen, zaakobjecten, zaakeigenschappen, zaakkenmerken, klantcontacten
        ///
        /// We must manually delete:
        /// - Besluiten
        /// - The actual documents (enkelvoudiginformatieobjecten)
        /// </summary>
        private async Task DeleteExistingZaakAndRelatedObjectsAsync(OzZaak zaak, CancellationToken token)
        {
            var zaakInformatieobjecten = await _openZaakApiClient.GetZaakInformatieobjectenForZaak(zaak.Url);
            var besluiten = await _openZaakApiClient.GetBesluitenForZaak(zaak.Url);
            foreach (var besluit in besluiten)
            {
                var besluitId = OzUrlToGuidConverter.ExtractUuidFromUrl(besluit.Url.ToString());
                await _openZaakApiClient.DeleteBesluit(besluitId);
            }

            var zaakId = OzUrlToGuidConverter.ExtractUuidFromUrl(zaak.Url.ToString());
            await _openZaakApiClient.DeleteZaak(zaakId);

            // finally delete documents
            foreach (var zio in zaakInformatieobjecten)
            {
                var documentId = OzUrlToGuidConverter.ExtractUuidFromUrl(zio.Informatieobject);
                await _openZaakApiClient.DeleteDocument(documentId);
            }
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

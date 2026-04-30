using System.Diagnostics;
using System.Diagnostics.Metrics;
using Datamigratie.Common.Helpers;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Pdf;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak
{
    public interface IMigrateZaakService
    {
        public Task<MigrateZaakResult> MigrateZaak(string zaaknummer, Models.Mappers mapping, CancellationToken token = default);
    }

    public class MigrateZaakService(
        IOpenZaakApiClient openZaakApiClient,
        IDetApiClient detClient,
        IZaakgegevensPdfGenerator pdfGenerator,
        IZaakDocumentMigrator zaakDocumentMigrator,
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

        public async Task<MigrateZaakResult> MigrateZaak(string zaaknummer, Models.Mappers mapping, CancellationToken token = default)
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
                var zaakRequest = mapping.ZaakMapper.Map(detZaak);


                // Check if zaken with the same identificatie already exist in OpenZaak and delete all matches to allow re-run
                var existingZaken = await _openZaakApiClient.GetZakenByIdentificatie(detZaak.FunctioneleIdentificatie);
                foreach (var existingZaak in existingZaken)
                {
                    await DeleteExistingZaakAndRelatedObjectsAsync(existingZaak, token);
                }

                OzZaak createdZaak;
                using (ActivitySource.StartActivity("CreateZaak"))
                {
                    createdZaak = await _openZaakApiClient.CreateZaak(zaakRequest);
                }

                var resultaatRequest = detZaak.Resultaat == null
                    ? null
                    : mapping.ResultaatMapper.Map(detZaak.Resultaat, createdZaak.Url);

                var statusRequest = detZaak.ZaakStatus == null
                    ? null
                    : mapping.StatusMapper.Map(detZaak.ZaakStatus, detZaak, createdZaak.Url);

                var pdfRequest = mapping.PdfMapper.Map(detZaak);

                var documentRequests = detZaak.Documenten?
                    .Select(mapping.DocumentMapper.Map)
                    .ToList() ?? [];

                var besluitRequests = detZaak.Besluiten?
                    .Select(x => mapping.BesluitMapper.Map(x, createdZaak.Url))
                    .ToList() ?? [];

                var rolRequests = mapping.RolMapper.MapRoles(detZaak, createdZaak.Url).ToList();

                // Create resultaat for the zaak based on resultaat mapping (must be run before status)
                await CreateResultaatAsync(resultaatRequest, token);

                // Create status for the zaak based on status mapping
                await ExecuteStatusPlanAsync(statusRequest, token);

                // Generate and upload the zaakgegevens PDF
                await ExecutePdfPlanAsync(pdfRequest, detZaak, createdZaak, token);

                // Migrate all documents with their versions
                await SaveDocumentsAsync(documentRequests, createdZaak, token);

                // Migrate all besluiten for the zaak
                await SaveBesluitenAsync(besluitRequests, createdZaak, token);

                // Migrate rollen (e.g. Behandelaar) to OpenZaak
                await ExecuteRolPlansAsync(rolRequests, createdZaak, token);

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


        private async Task ExecutePdfPlanAsync(OzDocument ozDocument, DetZaak detZaak, OzZaak createdZaak, CancellationToken token)
        {
            using var activity = ActivitySource.StartActivity("UploadZaakgegevensPdf");

            using var stream = new MemoryStream();
            using (ActivitySource.StartActivity("GenerateZaakgegevensPdf"))
            {
                pdfGenerator.GenerateZaakgegevensPdf(detZaak, stream);
            }
            stream.Seek(0, SeekOrigin.Begin);

            // Create document with the actual PDF size
            ozDocument.Bestandsomvang = stream.Length;

            await zaakDocumentMigrator.CreateAndLinkDocumentAsync(
                ozDocument,
                createdZaak,
                stream,
                token);
        }

        private async Task CreateResultaatAsync(CreateOzResultaatRequest? plan, CancellationToken token)
        {
            if (plan == null) return;

            using var activity = ActivitySource.StartActivity("MigrateResultaat");
            await _openZaakApiClient.CreateResultaat(plan);
        }

        private async Task ExecuteStatusPlanAsync(CreateOzStatusRequest? plan, CancellationToken token)
        {
            if (plan == null) return;

            using var activity = ActivitySource.StartActivity("MigrateStatus");

            await _openZaakApiClient.CreateStatus(plan);
        }

        private async Task ExecuteRolPlansAsync(IReadOnlyCollection<OzCreateRolRequest> rollen, OzZaak createdZaak, CancellationToken token)
        {
            List<OzRoltype>? ozRoltypes = null;
            foreach (var rol in rollen)
            {
                try
                {
                    await _openZaakApiClient.CreateRol(rol, token);
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("max-occurences", StringComparison.OrdinalIgnoreCase))
                {
                    ozRoltypes ??= await _openZaakApiClient.GetRoltypesForZaaktype(createdZaak.Zaaktype);
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
        private async Task SaveBesluitenAsync(IReadOnlyList<CreateOzBesluitRequest> besluitPlans, OzZaak createdZaak, CancellationToken token)
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
                    throw new Exception(
                        $"Migratie onderbroken: besluit '{ozBesluitRequest.Identificatie}' kon niet worden gemigreerd{FormatHttpStatusInfo(ex)}",
                        ex);
                }
            }
        }

        /// <summary>
        /// Migrates all documents with their versions in the correct order.
        /// First version is created, next versions update the same document (OpenZaak auto-increments version).
        /// </summary>
        private async Task SaveDocumentsAsync(List<DocumentVersions> documentPlans, OzZaak createdZaak, CancellationToken token)
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
                            await detClient.GetDocumentInhoudAsync(versionPlan.DetInhoudId, async (stream, ct) =>
                            {
                                mainDocument = await zaakDocumentMigrator.CreateAndLinkDocumentAsync(versionPlan.Document, createdZaak, stream, ct);
                            }, token);
                        }
                        else
                        {
                            // other versions: update existing document
                            if (mainDocument?.Id == null)
                            {
                                throw new InvalidOperationException("First document version must be created before updating");
                            }

                            await detClient.GetDocumentInhoudAsync(versionPlan.DetInhoudId, async (stream, ct) =>
                            {
                                await zaakDocumentMigrator.UpdateDocumentVersionAsync(mainDocument.Id, versionPlan.Document, stream, ct);
                            }, token);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            $"Migratie onderbroken: versie {i + 1} van document '{versionPlan.Document.Titel}' (bestand: {versionPlan.Document.Bestandsnaam}) kon niet worden gemigreerd{FormatHttpStatusInfo(ex)}",
                            ex);
                    }
                }
            }
        }

        private static string FormatHttpStatusInfo(Exception ex) => ExceptionFormatter.FormatHttpStatusInfo(ex);

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
                if (string.IsNullOrEmpty(zio.Informatieobject))
                {
                    logger.LogWarning("Skipping document deletion: zaakinformatieobject linked to zaak {ZaakUrl} has a null/empty informatieobject URL (broken link in OpenZaak)", zaak.Url);
                    continue;
                }

                logger.LogInformation("Deleting document with url {DocumentUrl} linked to existing zaak {ZaakUrl}",
                    zio.Informatieobject, zaak.Url);
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

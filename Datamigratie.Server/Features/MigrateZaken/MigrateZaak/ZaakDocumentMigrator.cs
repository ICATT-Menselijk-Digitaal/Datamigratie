using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak;


public interface IZaakDocumentMigrator
{
    Task<OzDocument> CreateAndLinkDocumentAsync(OzDocument ozDocument, OzZaak zaak, Stream stream, CancellationToken token);
    Task UpdateDocumentVersionAsync(Guid documentId, OzDocument ozDocument, Stream stream, CancellationToken token);
}

public class ZaakDocumentMigrator(IOpenZaakApiClient openZaakApiClient, ILogger<ZaakDocumentMigrator> logger) : IZaakDocumentMigrator
{
    public async Task<OzDocument> CreateAndLinkDocumentAsync(
            OzDocument ozDocument,
            OzZaak zaak,
            Stream stream,
            CancellationToken token)
    {
        var savedDocument = await openZaakApiClient.CreateDocument(ozDocument);

        try
        {
            await openZaakApiClient.KoppelDocument(zaak, savedDocument, token);
            await openZaakApiClient.UploadBestand(savedDocument, stream, token);
        }
        catch
        {
            // if the document was created but failed to link to the zaak, we should delete the document to avoid orphan documents in OpenZaak.
            // We swallow any errors during deletion to not mask the original exception that caused the linking to fail, but we log it just in case.
            // before we can delete we need to unlock
            await TryUnlockDocumentIgnoringErrorsAsync(savedDocument.Id, savedDocument.Lock, token);
            await TryDeleteDocumentIgnoringErrorsAsync(savedDocument.Id);
            throw;
        }

        await openZaakApiClient.UnlockDocument(savedDocument.Id, savedDocument.Lock, token);

        return savedDocument;
    }

    public async Task UpdateDocumentVersionAsync(Guid documentId, OzDocument ozDocument, Stream stream, CancellationToken token)
    {
        var lockToken = await openZaakApiClient.LockDocument(documentId, token);

        try
        {
            ozDocument.Lock = lockToken;

            // update document to create new version
            await openZaakApiClient.UpdateDocument(documentId, ozDocument);

            // after an update the document contains outdated bestandsdelen information.
            // we need to GET a document again in order to get the latest bestandsdelen
            var refreshedDocument = await openZaakApiClient.GetDocument(documentId)
                ?? throw new InvalidDataException($"We cannot find the document with id {documentId} that was updated.");

            refreshedDocument.Lock = lockToken;

            await openZaakApiClient.UploadBestand(refreshedDocument, stream, token);
        }
        catch
        {
            await TryUnlockDocumentIgnoringErrorsAsync(documentId, lockToken, token);
            throw;
        }
        await openZaakApiClient.UnlockDocument(documentId, lockToken, token);
    }

    private async Task TryDeleteDocumentIgnoringErrorsAsync(Guid documentId)
    {
        try
        {
            await openZaakApiClient.DeleteDocument(documentId);
        }
        catch (Exception ex)
        {
            // Swallow delete failures so the original exception propagates that triggered this delete attempt
            logger.LogError(ex, "Failed to delete document {DocumentId} after an error. The document may remain in OpenZaak.", documentId);
        }
    }

    private async Task TryUnlockDocumentIgnoringErrorsAsync(Guid documentId, string? lockToken, CancellationToken token)
    {
        try
        {
            await openZaakApiClient.UnlockDocument(documentId, lockToken, token);
        }
        catch (Exception ex)
        {
            // Swallow unlock failures so the original exception propagates that triggered this unlock attempt
            logger.LogError(ex, "Failed to unlock document {DocumentId} after an error. The document may remain locked in OpenZaak.", documentId);
        }
    }
}

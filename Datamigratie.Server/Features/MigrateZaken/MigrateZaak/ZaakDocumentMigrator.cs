using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak;

public interface IZaakDocumentMigrator
{
    Task<OzDocument> CreateAndLinkDocumentAsync(OzDocument ozDocument, OzZaak zaak, Stream stream, CancellationToken token);
    Task<OzDocument> UpdateDocumentVersionAsync(Guid documentId, OzDocument ozDocument, Stream stream, CancellationToken token);
}

public class ZaakDocumentMigrator(IOpenZaakApiClient openZaakApiClient) : IZaakDocumentMigrator
{
    public async Task<OzDocument> CreateAndLinkDocumentAsync(
            OzDocument ozDocument,
            OzZaak zaak,
            Stream stream,
            CancellationToken token)
    {
        var workflow = new WorkflowStep<OzDocument, OzDocument>(
            run: openZaakApiClient.CreateDocument,
            compensate: async (_, doc) =>
            {
                try
                {
                    await openZaakApiClient.UnlockDocument(doc.Id, doc.Lock, token);
                    await openZaakApiClient.DeleteDocument(doc.Id);
                }
                catch (Exception ex)
                {
                    throw new OrphanedDocumentException(doc.Id, ex);
                }
            })
            .Then((doc) => openZaakApiClient.KoppelDocument(zaak, doc, token))
            .Then((doc) => openZaakApiClient.UploadBestand(doc, stream, token))
            .Then((doc) => openZaakApiClient.UnlockDocument(doc.Id, doc.Lock, token));

        return await workflow.Run(ozDocument);
    }

    public async Task<OzDocument> UpdateDocumentVersionAsync(Guid documentId, OzDocument ozDocument, Stream stream, CancellationToken token)
    {
        var workflow = new WorkflowStep<Guid, OzDocument>(
            run: async id =>
            {
                ozDocument.Lock = await openZaakApiClient.LockDocument(id, token);
                return ozDocument;
            },
            compensate: (id, doc) => openZaakApiClient.UnlockDocument(id, doc.Lock, token))
            .Then(async doc =>
            {
                await openZaakApiClient.UpdateDocument(documentId, doc);
                return doc;
            })
            .Then(async (doc) =>
            {
                var refreshedDocument = await openZaakApiClient.GetDocument(documentId)
                    ?? throw new InvalidDataException($"We cannot find the document with id {documentId} that was updated.");
                refreshedDocument.Lock = doc.Lock; // Preserve the lock for the next steps
                return refreshedDocument;
            })
            .Then(doc => openZaakApiClient.UploadBestand(doc, stream, token))
            .Then(doc => openZaakApiClient.UnlockDocument(documentId, doc.Lock, token));

        return await workflow.Run(documentId);
    }
}

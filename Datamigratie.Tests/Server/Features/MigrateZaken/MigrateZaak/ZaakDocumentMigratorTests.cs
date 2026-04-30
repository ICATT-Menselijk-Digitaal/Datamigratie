using Datamigratie.Common.Services.OpenZaak;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Moq;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak;

public class ZaakDocumentMigratorTests
{
    private const string IotypeUrl = "https://openzaak.example.com/catalogi/api/v1/informatieobjecttypen/iotype-uuid";
    private const string DocumentUrl = "https://openzaak.example.com/documenten/api/v1/enkelvoudiginformatieobjecten/00000000-0000-0000-0000-000000000001";
    private static readonly Guid DocumentId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    private static OzDocument CreateOzDocument() => new()
    {
        Url = DocumentUrl,
        Bestandsnaam = "test.pdf",
        Bestandsomvang = 100,
        Bestandsdelen = [],
        Bronorganisatie = "123456789",
        Creatiedatum = DateOnly.FromDateTime(DateTime.Today),
        Titel = "Test",
        Vertrouwelijkheidaanduiding = DocumentVertrouwelijkheidaanduiding.openbaar,
        Auteur = "test",
        Status = DocumentStatus.definitief,
        Taal = "nld",
        Link = "",
        Beschrijving = "",
        Verschijningsvorm = "",
        Informatieobjecttype = new Uri(IotypeUrl),
        Trefwoorden = [],
        Lock = "lock-token-123"
    };

    private static OzZaak CreateOzZaak() => new()
    {
        Url = new Uri("https://openzaak.example.com/zaken/api/v1/zaken/zaak-uuid"),
        Zaaktype = new Uri("https://openzaak.example.com/catalogi/api/v1/zaaktypen/zaaktype-uuid")
    };

    private static Mock<IOpenZaakApiClient> CreateClientMock()
    {
        var mock = new Mock<IOpenZaakApiClient>();

        mock.Setup(c => c.CreateDocument(It.IsAny<OzDocument>()))
            .ReturnsAsync(CreateOzDocument());

        mock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.DeleteDocument(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        mock.Setup(c => c.LockDocument(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("lock-token-456");

        mock.Setup(c => c.UpdateDocument(It.IsAny<Guid>(), It.IsAny<OzDocument>()))
            .ReturnsAsync(CreateOzDocument());

        mock.Setup(c => c.GetDocument(It.IsAny<Guid>()))
            .ReturnsAsync(CreateOzDocument());

        return mock;
    }

    private static ZaakDocumentMigrator CreateMigrator(Mock<IOpenZaakApiClient> clientMock) =>
        new(clientMock.Object);

    #region CreateAndLinkDocumentAsync — happy path

    [Fact]
    public async Task CreateAndLink_HappyPath_CreatesDocumentThenLinksUploadsAndUnlocks()
    {
        var callOrder = new List<string>();
        var clientMock = CreateClientMock();

        clientMock.Setup(c => c.CreateDocument(It.IsAny<OzDocument>()))
            .Callback(() => callOrder.Add("CreateDocument"))
            .ReturnsAsync(CreateOzDocument());
        clientMock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("KoppelDocument"))
            .Returns(Task.CompletedTask);
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UploadBestand"))
            .Returns(Task.CompletedTask);
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UnlockDocument"))
            .Returns(Task.CompletedTask);

        var migrator = CreateMigrator(clientMock);

        await migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None);

        Assert.Equal(["CreateDocument", "KoppelDocument", "UploadBestand", "UnlockDocument"], callOrder);
    }

    [Fact]
    public async Task CreateAndLink_HappyPath_ReturnsSavedDocument()
    {
        var clientMock = CreateClientMock();
        var migrator = CreateMigrator(clientMock);

        var result = await migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None);

        Assert.Equal(DocumentId, result.Id);
    }

    [Fact]
    public async Task CreateAndLink_HappyPath_DoesNotDeleteDocument()
    {
        var clientMock = CreateClientMock();
        var migrator = CreateMigrator(clientMock);

        await migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None);

        clientMock.Verify(c => c.DeleteDocument(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region CreateAndLinkDocumentAsync — CreateDocument fails

    [Fact]
    public async Task CreateAndLink_WhenCreateDocumentFails_DoesNotCompensate()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.CreateDocument(It.IsAny<OzDocument>()))
            .ThrowsAsync(new HttpRequestException("Create failed"));

        var migrator = CreateMigrator(clientMock);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        clientMock.Verify(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
        clientMock.Verify(c => c.DeleteDocument(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region CreateAndLinkDocumentAsync — KoppelDocument fails

    [Fact]
    public async Task CreateAndLink_WhenKoppelFails_DeletesCreatedDocument()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Linking failed"));

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Equal("Linking failed", ex.Message);
        clientMock.Verify(c => c.DeleteDocument(DocumentId), Times.Once);
    }

    [Fact]
    public async Task CreateAndLink_WhenKoppelFails_DoesNotUploadContent()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Linking failed"));

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Equal("Linking failed", ex.Message);
        clientMock.Verify(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAndLink_WhenKoppelFails_UnlocksBeforeDeleting()
    {
        var callOrder = new List<string>();
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Linking failed"));
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UnlockDocument"))
            .Returns(Task.CompletedTask);
        clientMock.Setup(c => c.DeleteDocument(It.IsAny<Guid>()))
            .Callback(() => callOrder.Add("DeleteDocument"))
            .Returns(Task.CompletedTask);

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Equal("Linking failed", ex.Message);
        Assert.Equal(["UnlockDocument", "DeleteDocument"], callOrder);
    }

    [Fact]
    public async Task CreateAndLink_WhenKoppelFailsAndDeleteAlsoFails_RethrowsOriginalException()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.KoppelDocument(It.IsAny<OzZaak>(), It.IsAny<OzDocument>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Linking failed"));
        clientMock.Setup(c => c.DeleteDocument(It.IsAny<Guid>()))
            .ThrowsAsync(new HttpRequestException("Delete also failed"));

        var migrator = CreateMigrator(clientMock);

        var agg = await Assert.ThrowsAsync<AggregateException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Equal(2, agg.InnerExceptions.Count);
        var first = Assert.IsType<HttpRequestException>(agg.InnerExceptions[0]);
        Assert.Equal("Linking failed", first.Message);
        var orphaned = Assert.IsType<OrphanedDocumentException>(agg.InnerExceptions[1]);
        Assert.Equal(DocumentId, orphaned.DocumentId);
    }

    #endregion

    #region CreateAndLinkDocumentAsync — upload fails

    [Fact]
    public async Task CreateAndLink_WhenUploadFails_UnlocksDocument()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Upload failed"));

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Equal("Upload failed", ex.Message);
        clientMock.Verify(c => c.UnlockDocument(DocumentId, "lock-token-123", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAndLink_WhenUploadFailsAndUnlockAlsoFails_RethrowsOriginalException()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Upload failed"));
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Unlock also failed"));

        var migrator = CreateMigrator(clientMock);

        var agg = await Assert.ThrowsAsync<AggregateException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Contains(agg.InnerExceptions, ex => ex is HttpRequestException { Message: "Upload failed" });
        var orphaned = Assert.Single(agg.InnerExceptions.OfType<OrphanedDocumentException>());
        Assert.Equal(DocumentId, orphaned.DocumentId);
    }

    #endregion

    #region CreateAndLinkDocumentAsync — UnlockDocument (final step) fails

    [Fact]
    public async Task CreateAndLink_WhenFinalUnlockFails_CompensatesAndRethrows()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Unlock failed"));

        var migrator = CreateMigrator(clientMock);

        var agg = await Assert.ThrowsAsync<AggregateException>(() =>
            migrator.CreateAndLinkDocumentAsync(CreateOzDocument(), CreateOzZaak(), Stream.Null, CancellationToken.None));

        Assert.Contains(agg.InnerExceptions, ex => ex is HttpRequestException { Message: "Unlock failed" });
        var orphaned = Assert.Single(agg.InnerExceptions.OfType<OrphanedDocumentException>());
        Assert.Equal(DocumentId, orphaned.DocumentId);
    }

    #endregion

    #region UpdateDocumentVersionAsync — happy path

    [Fact]
    public async Task UpdateVersion_HappyPath_LocksUpdatesUploadsAndUnlocks()
    {
        var callOrder = new List<string>();
        var clientMock = CreateClientMock();

        clientMock.Setup(c => c.LockDocument(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("LockDocument"))
            .ReturnsAsync("lock-token-456");
        clientMock.Setup(c => c.UpdateDocument(It.IsAny<Guid>(), It.IsAny<OzDocument>()))
            .Callback(() => callOrder.Add("UpdateDocument"))
            .ReturnsAsync(CreateOzDocument());
        clientMock.Setup(c => c.GetDocument(It.IsAny<Guid>()))
            .Callback(() => callOrder.Add("GetDocument"))
            .ReturnsAsync(CreateOzDocument());
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UploadBestand"))
            .Returns(Task.CompletedTask);
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("UnlockDocument"))
            .Returns(Task.CompletedTask);

        var migrator = CreateMigrator(clientMock);

        await migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None);

        Assert.Equal(["LockDocument", "UpdateDocument", "GetDocument", "UploadBestand", "UnlockDocument"], callOrder);
    }

    [Fact]
    public async Task UpdateVersion_HappyPath_SetsLockTokenOnDocument()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.LockDocument(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("my-lock-token");

        var inputDoc = CreateOzDocument();
        var migrator = CreateMigrator(clientMock);

        await migrator.UpdateDocumentVersionAsync(DocumentId, inputDoc, Stream.Null, CancellationToken.None);

        Assert.Equal("my-lock-token", inputDoc.Lock);
    }

    [Fact]
    public async Task UpdateVersion_HappyPath_UploadsToRefreshedDocument()
    {
        var refreshedDoc = CreateOzDocument();
        refreshedDoc.Bestandsomvang = 999;

        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.GetDocument(DocumentId))
            .ReturnsAsync(refreshedDoc);

        OzDocument? uploadedDoc = null;
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<OzDocument, Stream, CancellationToken>((doc, _, _) => uploadedDoc = doc)
            .Returns(Task.CompletedTask);

        var migrator = CreateMigrator(clientMock);

        await migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None);

        Assert.Equal(999, uploadedDoc?.Bestandsomvang);
    }

    #endregion

    #region UpdateDocumentVersionAsync — failures

    [Fact]
    public async Task UpdateVersion_WhenLockFails_DoesNotCompensate()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.LockDocument(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Lock failed"));

        var migrator = CreateMigrator(clientMock);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None));

        clientMock.Verify(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
        clientMock.Verify(c => c.UpdateDocument(It.IsAny<Guid>(), It.IsAny<OzDocument>()), Times.Never);
    }

    [Fact]
    public async Task UpdateVersion_WhenUpdateDocumentFails_UnlocksDocument()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UpdateDocument(It.IsAny<Guid>(), It.IsAny<OzDocument>()))
            .ThrowsAsync(new HttpRequestException("Update failed"));

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None));

        Assert.Equal("Update failed", ex.Message);
        clientMock.Verify(c => c.UnlockDocument(DocumentId, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_WhenGetDocumentReturnsNull_ThrowsInvalidDataException()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.GetDocument(It.IsAny<Guid>()))
            .ReturnsAsync((OzDocument?)null);

        var migrator = CreateMigrator(clientMock);

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateVersion_WhenUploadFails_UnlocksDocument()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Upload failed"));

        var migrator = CreateMigrator(clientMock);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None));

        Assert.Equal("Upload failed", ex.Message);
        clientMock.Verify(c => c.UnlockDocument(DocumentId, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_WhenUploadFailsAndUnlockAlsoFails_RethrowsOriginalException()
    {
        var clientMock = CreateClientMock();
        clientMock.Setup(c => c.UploadBestand(It.IsAny<OzDocument>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Upload failed"));
        clientMock.Setup(c => c.UnlockDocument(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Unlock failed"));

        var migrator = CreateMigrator(clientMock);

        var agg = await Assert.ThrowsAsync<AggregateException>(() =>
            migrator.UpdateDocumentVersionAsync(DocumentId, CreateOzDocument(), Stream.Null, CancellationToken.None));

        Assert.Equal(2, agg.InnerExceptions.Count);
        var first = Assert.IsType<HttpRequestException>(agg.InnerExceptions[0]);
        Assert.Equal("Upload failed", first.Message);
        var second = Assert.IsType<HttpRequestException>(agg.InnerExceptions[1]);
        Assert.Equal("Unlock failed", second.Message);
    }

    #endregion
}

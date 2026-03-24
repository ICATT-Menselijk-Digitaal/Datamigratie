using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

public class StartSingleMigrationControllerTests
{
    private const string ZaaktypeId = "zaaktype-001";
    private const string Zaaknummer = "zaak-001";

    private static DetZaak ClosedZaakForType(string zaaktypeId = ZaaktypeId) => new()
    {
        FunctioneleIdentificatie = Zaaknummer,
        Open = false,
        Omschrijving = "Test zaak",
        Zaaktype = new DetZaaktype
        {
            FunctioneleIdentificatie = zaaktypeId,
            Naam = "Test zaaktype",
            Omschrijving = "Test"
        }
    };

    private static StartSingleMigrationRequest DefaultRequest => new()
    {
        DetZaaktypeId = ZaaktypeId,
        Zaaknummer = Zaaknummer
    };

    private static MigrationQueueItem CreateQueueItem() => new()
    {
        DetZaaktypeId = ZaaktypeId,
        ZakenSelector = new SingleZaakSelector(Zaaknummer, open: false),
        RsinMapping = new RsinMapping { Rsin = "000000000" },
        StatusMappings = [],
        ResultaatMappings = [],
        DocumentstatusMappings = [],
        PublicatieNiveauMappings = [],
        DocumenttypeMappings = [],
        ZaakVertrouwelijkheidMappings = [],
        BesluittypeMappings = [],
        PdfInformatieobjecttypeId = Guid.Empty,
        RoltypeMappings = []
    };

    private static StartSingleMigrationController CreateSut(
        MigrationWorkerState workerState,
        Mock<IDetApiClient> detClientMock,
        Mock<IBuildMigrationQueueItemService>? buildServiceMock = null,
        Mock<IMigrationBackgroundTaskQueue>? queueMock = null)
    {
        buildServiceMock ??= new Mock<IBuildMigrationQueueItemService>();
        queueMock ??= new Mock<IMigrationBackgroundTaskQueue>();
        return new StartSingleMigrationController(
            workerState,
            queueMock.Object,
            buildServiceMock.Object,
            detClientMock.Object);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsConflict_WhenWorkerIsAlreadyRunning()
    {
        var workerState = new MigrationWorkerState { IsWorking = true };
        var sut = CreateSut(workerState, new Mock<IDetApiClient>());

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsUnprocessableEntity_WhenZaakNotFoundInDet()
    {
        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer)).ReturnsAsync((DetZaak?)null);

        var sut = CreateSut(new MigrationWorkerState(), detClientMock);

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsUnprocessableEntity_WhenZaakBelongsToDifferentZaaktype()
    {
        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer))
            .ReturnsAsync(ClosedZaakForType("zaaktype-other"));

        var sut = CreateSut(new MigrationWorkerState(), detClientMock);

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsUnprocessableEntity_WhenZaakIsOpen()
    {
        var openZaak = ClosedZaakForType();
        openZaak.Open = true;

        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer)).ReturnsAsync(openZaak);

        var sut = CreateSut(new MigrationWorkerState(), detClientMock);

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsOk_WhenZaakIsValidAndClosed()
    {
        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer)).ReturnsAsync(ClosedZaakForType());

        var buildServiceMock = new Mock<IBuildMigrationQueueItemService>();
        buildServiceMock.Setup(s => s.ValidateAndBuildAsync(ZaaktypeId, It.IsAny<IZakenSelector>()))
            .ReturnsAsync(CreateQueueItem());

        var queueMock = new Mock<IMigrationBackgroundTaskQueue>();
        queueMock.Setup(q => q.QueueMigrationAsync(It.IsAny<MigrationQueueItem>()))
            .Returns(ValueTask.CompletedTask);

        var sut = CreateSut(new MigrationWorkerState(), detClientMock, buildServiceMock, queueMock);

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_ReturnsConflict_WhenDetThrowsUnexpectedException()
    {
        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer))
            .ThrowsAsync(new HttpRequestException("DET service unavailable"));

        var sut = CreateSut(new MigrationWorkerState(), detClientMock);

        var result = await sut.StartSingleMigration(DefaultRequest);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task StartSingleMigration_QueuesMigration_WhenZaakIsValidAndClosed()
    {
        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZaakByZaaknummer(Zaaknummer)).ReturnsAsync(ClosedZaakForType());

        var expectedQueueItem = CreateQueueItem();
        var buildServiceMock = new Mock<IBuildMigrationQueueItemService>();
        buildServiceMock.Setup(s => s.ValidateAndBuildAsync(ZaaktypeId, It.IsAny<IZakenSelector>()))
            .ReturnsAsync(expectedQueueItem);

        var queueMock = new Mock<IMigrationBackgroundTaskQueue>();
        queueMock.Setup(q => q.QueueMigrationAsync(It.IsAny<MigrationQueueItem>()))
            .Returns(ValueTask.CompletedTask);

        var sut = CreateSut(new MigrationWorkerState(), detClientMock, buildServiceMock, queueMock);

        await sut.StartSingleMigration(DefaultRequest);

        queueMock.Verify(q => q.QueueMigrationAsync(expectedQueueItem), Times.Once);
    }
}

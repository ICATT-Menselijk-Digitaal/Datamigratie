using System.Net;
using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartFullMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

public class StartMigrationServiceTests
{
    private const string ZaaktypeId = "zaaktype-001";

    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static void SeedZaaktypeMapping(DatamigratieDbContext context, string zaaktypeId)
    {
        context.Mappings.Add(new ZaaktypenMapping
        {
            DetZaaktypeId = zaaktypeId,
            OzZaaktypeId = Guid.NewGuid()
        });
        context.SaveChanges();
    }

    private static MigrationQueueItem CreateQueueItem(IZakenSelector selector)
        => new()
        {
            DetZaaktypeId = ZaaktypeId,
            ZakenSelector = selector,
            RsinMapping = new RsinMapping { Rsin = "000000000" },
            StatusMappings = new(),
            ResultaatMappings = new(),
            DocumentstatusMappings = new(),
            PublicatieNiveauMappings = new(),
            DocumenttypeMappings = new(),
            ZaakVertrouwelijkheidMappings = new(),
            BesluittypeMappings = new(),
            PdfInformatieobjecttypeId = Guid.NewGuid(),
            RoltypeMappings = new(),
        };

    private static DetZaak CreateDetZaak(string zaaknummer)
        => new()
        {
            FunctioneleIdentificatie = zaaknummer,
            Open = false,
            Omschrijving = "test",
            CreatieDatumTijd = DateTimeOffset.UtcNow,
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            Streefdatum = DateOnly.FromDateTime(DateTime.Today),
            Historie = [],
        };

    private static StartMigrationService CreateSut(
        DatamigratieDbContext context,
        Mock<IDetApiClient> detClient,
        Mock<IMigrateZaakService> migrateZaakService)
    {
        var options = Options.Create(new OpenZaakApiOptions { BaseUrl = "https://openzaak.test/" });
        return new StartMigrationService(
            context,
            detClient.Object,
            NullLogger<StartMigrationService>.Instance,
            migrateZaakService.Object,
            new MigrationWorkerState(),
            options);
    }

    // --- Group 1: Migration type branching ---

    [Fact]
    public async Task PerformMigrationAsync_FullMigration_OnlyMigratesClosedZakenAndSetsTotalRecords()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId)).ReturnsAsync([
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = true },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = false },
        ]);
        detClient.Setup(c => c.GetZaakByZaaknummer(It.IsAny<string>()))
            .ReturnsAsync((string id) => CreateDetZaak(id));

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        // Use a FullMigrationZakenSelector with the mocked detClient
        var selector = new FullMigrationZakenSelector(detClient.Object);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector), CancellationToken.None);

        // Assert: only closed zaken (zaak-001 and zaak-003) are migrated
        migrateZaak.Verify(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(2, migration.TotalRecords);
    }

    [Fact]
    public async Task PerformMigrationAsync_PartialMigration_OnlyMigratesSelectedZaken()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001"))
            .ReturnsAsync(CreateDetZaak("zaak-001"));

        // Use a custom selector that returns only zaak-001
        var selectorMock = new Mock<IZakenSelector>();
        selectorMock.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));


        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selectorMock.Object), CancellationToken.None);

        // Assert: selector called, DET bulk fetch NOT called, one zaak migrated
        selectorMock.Verify(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()), Times.Once);
        detClient.Verify(c => c.GetZakenByZaaktype(It.IsAny<string>()), Times.Never);
        migrateZaak.Verify(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Group 2: Record counter correctness ---

    [Fact]
    public async Task PerformMigrationAsync_SuccessfulZaak_IncrementsSuccessfulRecords()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001")).ReturnsAsync(CreateDetZaak("zaak-001"));

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(1, migration.SuccessfulRecords);
        Assert.Equal(0, migration.FailedRecords);

        var record = await context.MigrationRecords.FirstAsync();
        Assert.True(record.IsSuccessful);
        Assert.Equal("zaak-001", record.OzZaaknummer);
    }

    [Fact]
    public async Task PerformMigrationAsync_FailedZaak_IncrementsFailedRecords()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001")).ReturnsAsync(CreateDetZaak("zaak-001"));

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "fout", "details", 422));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(0, migration.SuccessfulRecords);
        Assert.Equal(1, migration.FailedRecords);

        var record = await context.MigrationRecords.FirstAsync();
        Assert.False(record.IsSuccessful);
        Assert.Equal("fout", record.ErrorTitle);
        Assert.Equal(422, record.StatusCode);
    }

    [Fact]
    public async Task PerformMigrationAsync_FailedZaakWithLongDetails_TruncatesErrorDetailsTo10000Characters()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001")).ReturnsAsync(CreateDetZaak("zaak-001"));

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "fout", new string('x', MigrationRecord.MaxErrorDetailsLength + 5000), 422));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert
        var record = await context.MigrationRecords.FirstAsync();
        Assert.Equal(MigrationRecord.MaxErrorDetailsLength, record.ErrorDetails!.Length);
    }

    [Fact]
    public async Task PerformMigrationAsync_HttpExceptionDuringFetch_CreatesFailedRecordWithHttpStatusCode()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001"))
            .ThrowsAsync(new HttpRequestException("Not found", null, HttpStatusCode.NotFound));

        var migrateZaak = new Mock<IMigrateZaakService>();

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert: failed record with 404 status code
        var record = await context.MigrationRecords.FirstAsync();
        Assert.False(record.IsSuccessful);
        Assert.Equal(404, record.StatusCode);

        // Migration itself completes (not failed)
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(MigrationStatus.Completed, migration.Status);
        Assert.Equal(1, migration.FailedRecords);
        Assert.Equal(1, migration.ProcessedRecords);
        Assert.Equal(1, migration.TotalRecords);
    }

    [Fact]
    public async Task PerformMigrationAsync_UnhandledExceptionDuringFetch_CreatesFailedRecordWith500()
    {
        // Arrange
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);
        detClient.Setup(c => c.GetZaakByZaaknummer("zaak-001"))
            .ThrowsAsync(new InvalidOperationException("Unexpected"));

        var migrateZaak = new Mock<IMigrateZaakService>();

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert: failed record with 500
        var record = await context.MigrationRecords.FirstAsync();
        Assert.False(record.IsSuccessful);
        Assert.Equal(500, record.StatusCode);

        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(MigrationStatus.Completed, migration.Status);
        Assert.Equal(1, migration.FailedRecords);
        Assert.Equal(1, migration.ProcessedRecords);
        Assert.Equal(1, migration.TotalRecords);
    }

    [Fact]
    public async Task PerformMigrationAsync_MixedResults_CountersMatchOutcomes()
    {
        // Arrange: 3 zaken — success, failure, success
        await using var context = CreateContext();
        SeedZaaktypeMapping(context, ZaaktypeId);

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId)).ReturnsAsync([
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = false },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = false },
        ]);
        detClient.Setup(c => c.GetZaakByZaaknummer(It.IsAny<string>()))
            .ReturnsAsync((string id) => CreateDetZaak(id));

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.SetupSequence(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-002", "fout", "details", 500))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-003", "ok"));

        var sut = CreateSut(context, detClient, migrateZaak);

        // Act
        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = false },
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = false },
            ]);
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(2, migration.SuccessfulRecords);
        Assert.Equal(1, migration.FailedRecords);
        Assert.Equal(3, migration.ProcessedRecords);
    }
}

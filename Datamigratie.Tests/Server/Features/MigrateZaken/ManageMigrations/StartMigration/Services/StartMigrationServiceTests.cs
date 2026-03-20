using System.Net;
using Datamigratie.Common.Config;
using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Models;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.Migrate.ManageMigrations.StartMigration.State;
using Datamigratie.Server.Features.Migrate.MigrateZaak;
using Datamigratie.Server.Features.Migrate.MigrateZaak.Models;
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

    private static MigrationQueueItem CreateQueueItem(MigrationType type = MigrationType.Full)
        => new()
        {
            DetZaaktypeId = ZaaktypeId,
            MigrationType = type,
            RsinMapping = new RsinMapping { Rsin = "000000000" },
            StatusMappings = [],
            ResultaatMappings = [],
            DocumentstatusMappings = [],
            DocumentPropertyMappings = [],
            ZaakVertrouwelijkheidMappings = [],
            BesluittypeMappings = [],
            PdfInformatieobjecttypeId = Guid.NewGuid()
        };

    private static DetZaak CreateDetZaak(string zaaknummer)
        => new()
        {
            FunctioneleIdentificatie = zaaknummer,
            Open = false,
            Omschrijving = "test",
            CreatieDatumTijd = DateTimeOffset.UtcNow,
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            Streefdatum = DateOnly.FromDateTime(DateTime.Today)
        };

    private static StartMigrationService CreateSut(
        DatamigratieDbContext context,
        Mock<IDetApiClient> detClient,
        Mock<IMigrateZaakService> migrateZaakService,
        Mock<IPartialMigrationZakenSelectionService> partialSelection)
    {
        var options = Options.Create(new OpenZaakApiOptions { BaseUrl = "https://openzaak.test/" });
        return new StartMigrationService(
            context,
            detClient.Object,
            NullLogger<StartMigrationService>.Instance,
            migrateZaakService.Object,
            new MigrationWorkerState(),
            options,
            partialSelection.Object);
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

        var partialSelection = new Mock<IPartialMigrationZakenSelectionService>();
        var sut = CreateSut(context, detClient, migrateZaak, partialSelection);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(MigrationType.Full), CancellationToken.None);

        // Assert: only closed zaken (zaak-001 and zaak-003) are migrated
        migrateZaak.Verify(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        partialSelection.Verify(s => s.SelectZakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
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

        var partialSelection = new Mock<IPartialMigrationZakenSelectionService>();
        partialSelection.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<DetZaak>(), It.IsAny<MigrateZaakMappingModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        var sut = CreateSut(context, detClient, migrateZaak, partialSelection);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(MigrationType.Partial), CancellationToken.None);

        // Assert: selection service called, DET bulk fetch NOT called, one zaak migrated
        partialSelection.Verify(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()), Times.Once);
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

        var sut = CreateSut(context, detClient, migrateZaak, new Mock<IPartialMigrationZakenSelectionService>());

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(), CancellationToken.None);

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

        var sut = CreateSut(context, detClient, migrateZaak, new Mock<IPartialMigrationZakenSelectionService>());

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(), CancellationToken.None);

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
        var sut = CreateSut(context, detClient, migrateZaak, new Mock<IPartialMigrationZakenSelectionService>());

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(), CancellationToken.None);

        // Assert: failed record with 404 status code
        var record = await context.MigrationRecords.FirstAsync();
        Assert.False(record.IsSuccessful);
        Assert.Equal(404, record.StatusCode);

        // Migration itself completes (not failed)
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(MigrationStatus.Completed, migration.Status);
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
        var sut = CreateSut(context, detClient, migrateZaak, new Mock<IPartialMigrationZakenSelectionService>());

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(), CancellationToken.None);

        // Assert: failed record with 500
        var record = await context.MigrationRecords.FirstAsync();
        Assert.False(record.IsSuccessful);
        Assert.Equal(500, record.StatusCode);

        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(MigrationStatus.Completed, migration.Status);
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

        var sut = CreateSut(context, detClient, migrateZaak, new Mock<IPartialMigrationZakenSelectionService>());

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(), CancellationToken.None);

        // Assert
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(2, migration.SuccessfulRecords);
        Assert.Equal(1, migration.FailedRecords);
        Assert.Equal(3, migration.ProcessedRecords);
    }
}

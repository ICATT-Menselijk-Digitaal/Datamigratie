using Datamigratie.Common.Services.Det;
using Datamigratie.Server.Config;
using Microsoft.Extensions.Options;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartFullMigration;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Queues.Items;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.State;
using Datamigratie.Common.Services.OpenZaak.Models;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Mappers;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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

    private static MigrationQueueItem CreateQueueItem(IZakenSelector selector)
        => new()
        {
            DetZaaktypeId = ZaaktypeId,
            ZakenSelector = selector,
            ResultaatMapper = new ResultaatMapper(new Dictionary<string, Uri>()),
            StatusMapper = new StatusMapper(new Dictionary<string, Uri>()),
            ZaakMapper = new ZaakMapper("000000000", new Uri("https://openzaak.test/catalogi/api/v1/zaaktypen/00000000-0000-0000-0000-000000000000"), new Dictionary<bool, ZaakVertrouwelijkheidaanduiding>()),
            DocumentMapper = new DocumentMapper(
                "000000000",
                new Dictionary<string, DocumentStatus>(),
                new Dictionary<string, DocumentVertrouwelijkheidaanduiding>(),
                new Dictionary<string, Uri>()),
            BesluitMapper = new BesluitMapper("000000000", new Dictionary<string, Uri>()),
            PdfMapper = new PdfMapper("000000000", new Uri("https://openzaak.test/catalogi/api/v1/informatieobjecttypen/00000000-0000-0000-0000-000000000000")),
            RolMapper = new([]),
        };

    private static StartMigrationService CreateSut(
        DatamigratieDbContext context,
        Mock<IMigrateZaakService> migrateZaakService)
    {
        var migrationOptions = Options.Create(new MigrationOptions { ZaakConcurrencyLimit = 1 });

        migrateZaakService
            .Setup(s => s.GetFirstInformatieObjectTypeUriAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Uri("https://openzaak.test/catalogi/api/v1/informatieobjecttypen/test-id"));

        return new StartMigrationService(
            context,
            NullLogger<StartMigrationService>.Instance,
            migrateZaakService.Object,
            new MigrationWorkerState(),
            migrationOptions);
    }

    // --- Group 1: Migration type branching ---

    [Fact]
    public async Task PerformMigrationAsync_FullMigration_OnlyMigratesClosedZakenAndSetsTotalRecords()
    {
        // Arrange
        await using var context = CreateContext();

        var detClient = new Mock<IDetApiClient>();
        detClient.Setup(c => c.GetZakenByZaaktype(ZaaktypeId)).ReturnsAsync([
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = true },
            new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = false },
        ]);

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        // Use a FullMigrationZakenSelector with the mocked detClient
        var selector = new FullMigrationZakenSelector(detClient.Object);

        var sut = CreateSut(context, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector), CancellationToken.None);

        // Assert: only closed zaken (zaak-001 and zaak-003) are migrated
        migrateZaak.Verify(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        var migration = await context.Migrations.FirstAsync();
        Assert.Equal(2, migration.TotalRecords);
    }

    [Fact]
    public async Task PerformMigrationAsync_PartialMigration_OnlyMigratesSelectedZaken()
    {
        // Arrange
        await using var context = CreateContext();

        // Use a custom selector that returns only zaak-001
        var selectorMock = new Mock<IZakenSelector>();
        selectorMock.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        var sut = CreateSut(context, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selectorMock.Object), CancellationToken.None);

        // Assert: selector called, one zaak migrated
        selectorMock.Verify(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()), Times.Once);
        migrateZaak.Verify(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- Group 2: Record counter correctness ---

    [Fact]
    public async Task PerformMigrationAsync_SuccessfulZaak_IncrementsSuccessfulRecords()
    {
        // Arrange
        await using var context = CreateContext();

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, migrateZaak);

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

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "fout", "details", 422));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, migrateZaak);

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

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "fout", new string('x', MigrationRecord.MaxErrorDetailsLength + 5000), 422));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, migrateZaak);

        // Act
        await sut.PerformMigrationAsync(CreateQueueItem(selector.Object), CancellationToken.None);

        // Assert
        var record = await context.MigrationRecords.FirstAsync();
        Assert.Equal(MigrationRecord.MaxErrorDetailsLength, record.ErrorDetails!.Length);
    }

    [Fact]
    public async Task PerformMigrationAsync_FetchFailure_CreatesFailedRecordWithStatusCode()
    {
        // Arrange
        await using var context = CreateContext();

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "De zaak kon niet opgehaald worden uit het bronsysteem.", "Not found", 404));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, migrateZaak);

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
    public async Task PerformMigrationAsync_UnhandledFailure_CreatesFailedRecordWith500()
    {
        // Arrange
        await using var context = CreateContext();

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.Setup(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-001", "Unexpected error", "Unexpected", 500));

        var selector = new Mock<IZakenSelector>();
        selector.Setup(s => s.SelectZakenAsync(ZaaktypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }]);

        var sut = CreateSut(context, migrateZaak);

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

        var migrateZaak = new Mock<IMigrateZaakService>();
        migrateZaak.SetupSequence(s => s.MigrateZaak(It.IsAny<string>(), It.IsAny<Mappers>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-001", "ok"))
            .ReturnsAsync(MigrateZaakResult.Failed("zaak-002", "fout", "details", 500))
            .ReturnsAsync(MigrateZaakResult.Success("zaak-003", "ok"));

        var sut = CreateSut(context, migrateZaak);

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

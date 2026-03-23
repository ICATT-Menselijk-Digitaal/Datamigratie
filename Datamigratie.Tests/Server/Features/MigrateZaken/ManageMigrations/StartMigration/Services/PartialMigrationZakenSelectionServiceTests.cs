using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartPartialMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

public class PartialMigrationZakenSelectionServiceTests
{
    private const string ZaaktypeId = "zaaktype-001";

    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static IServiceScopeFactory CreateScopeFactory(DatamigratieDbContext context)
    {
        var services = new ServiceCollection();
        services.AddSingleton(context);
        var provider = services.BuildServiceProvider();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(provider);
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);
        return scopeFactoryMock.Object;
    }

    private static Migration CreateMigration(DatamigratieDbContext context)
    {
        var migration = new Migration
        {
            DetZaaktypeId = ZaaktypeId,
            Status = MigrationStatus.Completed,
            LastUpdated = DateTime.UtcNow
        };
        context.Migrations.Add(migration);
        context.SaveChanges();
        return migration;
    }

    private static MigrationRecord CreateRecord(Migration migration, string zaaknummer, bool isSuccessful, DateTime processedAt)
        => new()
        {
            Migration = migration,
            MigrationId = migration.Id,
            DetZaaknummer = zaaknummer,
            IsSuccessful = isSuccessful,
            ProcessedAt = processedAt
        };

    [Fact]
    public async Task SelectZakenAsync_ReturnsNewlyClosedZaak_WhenNotPreviouslyAttempted()
    {
        // Arrange: one previous migration with one successful zaak; DET now has a new closed zaak
        await using var context = CreateContext();
        var migration = CreateMigration(context);
        context.MigrationRecords.Add(CreateRecord(migration, "zaak-001", isSuccessful: true, DateTime.UtcNow));
        await context.SaveChangesAsync();

        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false }, // already migrated
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = false }, // newly closed
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = true  }, // still open, skip
            ]);

        var scopeFactory = CreateScopeFactory(context);
        var sut = new PartialMigrationZakenSelector(scopeFactory, detClientMock.Object);

        // Act
        var result = await sut.SelectZakenAsync(ZaaktypeId);

        // Assert
        Assert.Single(result);
        Assert.Equal("zaak-002", result[0].FunctioneleIdentificatie);
    }

    [Fact]
    public async Task SelectZakenAsync_ReturnsFailedZaak_WhenLatestAttemptFailed()
    {
        // Arrange: zaak-001 failed, zaak-002 succeeded; DET has no new closed zaken
        await using var context = CreateContext();
        var migration = CreateMigration(context);
        context.MigrationRecords.AddRange(
            CreateRecord(migration, "zaak-001", isSuccessful: false, DateTime.UtcNow),
            CreateRecord(migration, "zaak-002", isSuccessful: true,  DateTime.UtcNow)
        );
        await context.SaveChangesAsync();

        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = false },
            ]);

        var scopeFactory = CreateScopeFactory(context);
        var sut = new PartialMigrationZakenSelector(scopeFactory, detClientMock.Object);

        // Act
        var result = await sut.SelectZakenAsync(ZaaktypeId);

        // Assert: only the failed zaak is re-selected
        Assert.Single(result);
        Assert.Equal("zaak-001", result[0].FunctioneleIdentificatie);
    }

    [Fact]
    public async Task SelectZakenAsync_ExcludesFailedZaak_WhenLatestAttemptSucceeded()
    {
        // Arrange: zaak-001 failed first, then succeeded in a later run
        await using var context = CreateContext();
        var migration1 = CreateMigration(context);
        var migration2 = CreateMigration(context);
        context.MigrationRecords.AddRange(
            CreateRecord(migration1, "zaak-001", isSuccessful: false, DateTime.UtcNow.AddHours(-2)),
            CreateRecord(migration2, "zaak-001", isSuccessful: true,  DateTime.UtcNow.AddHours(-1))
        );
        await context.SaveChangesAsync();

        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
            ]);

        var scopeFactory = CreateScopeFactory(context);
        var sut = new PartialMigrationZakenSelector(scopeFactory, detClientMock.Object);

        // Act
        var result = await sut.SelectZakenAsync(ZaaktypeId);

        // Assert: zaak recovered, nothing to re-run
        Assert.Empty(result);
    }

    [Fact]
    public async Task SelectZakenAsync_ReturnsBothFailedAndNewlyClosed_WhenBothApply()
    {
        // Arrange: zaak-001 still failing, zaak-002 succeeded, zaak-003 is newly closed
        await using var context = CreateContext();
        var migration = CreateMigration(context);
        context.MigrationRecords.AddRange(
            CreateRecord(migration, "zaak-001", isSuccessful: false, DateTime.UtcNow),
            CreateRecord(migration, "zaak-002", isSuccessful: true,  DateTime.UtcNow)
        );
        await context.SaveChangesAsync();

        var detClientMock = new Mock<IDetApiClient>();
        detClientMock.Setup(c => c.GetZakenByZaaktype(ZaaktypeId))
            .ReturnsAsync([
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-001", Open = false },
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-002", Open = false },
                new DetZaakMinimal { FunctioneleIdentificatie = "zaak-003", Open = false }, // newly closed
            ]);

        var scopeFactory = CreateScopeFactory(context);
        var sut = new PartialMigrationZakenSelector(scopeFactory, detClientMock.Object);

        // Act
        var result = await sut.SelectZakenAsync(ZaaktypeId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, z => z.FunctioneleIdentificatie == "zaak-001");
        Assert.Contains(result, z => z.FunctioneleIdentificatie == "zaak-003");
    }
}

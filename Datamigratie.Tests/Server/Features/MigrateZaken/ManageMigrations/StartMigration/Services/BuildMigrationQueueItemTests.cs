using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.Services;

public class BuildMigrationQueueItemTests
{
    [Fact]
    public void CreateDatamigratieKenmerk_SetsBronToDatamigratie()
    {
        var result = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("ZK-001");

        Assert.Equal("Datamigratie", result.Bron);
    }

    [Fact]
    public void CreateDatamigratieKenmerk_KenmerkDoesNotExceed40Characters()
    {
        var result = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("some-very-long-functional-identifier-that-exceeds-normal-length");

        Assert.True(result.Kenmerk.Length <= 40);
    }

    [Fact]
    public void CreateDatamigratieKenmerk_SameInputProducesSameOutput()
    {
        var result1 = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("ZK-002");
        var result2 = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("ZK-002");

        Assert.Equal(result1.Kenmerk, result2.Kenmerk);
    }

    [Fact]
    public void CreateDatamigratieKenmerk_DifferentInputProducesDifferentOutput()
    {
        var result1 = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("ZK-001");
        var result2 = BuildMigrationQueueItemService.CreateDatamigratieKenmerk("ZK-002");

        Assert.NotEqual(result1.Kenmerk, result2.Kenmerk);
    }
}

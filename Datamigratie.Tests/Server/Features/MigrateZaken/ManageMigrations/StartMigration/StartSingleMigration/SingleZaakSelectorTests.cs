using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.StartSingleMigration;

public class SingleZaakSelectorTests
{
    [Fact]
    public async Task SelectZakenAsync_ReturnsSingleZaak_WithCorrectZaaknummer()
    {
        var sut = new SingleZaakSelector("zaak-001", open: false);

        var result = await sut.SelectZakenAsync("zaaktype-001");

        Assert.Single(result);
        Assert.Equal("zaak-001", result[0].FunctioneleIdentificatie);
    }

    [Fact]
    public async Task SelectZakenAsync_PreservesOpen_WhenFalse()
    {
        var sut = new SingleZaakSelector("zaak-001", open: false);

        var result = await sut.SelectZakenAsync("zaaktype-001");

        Assert.False(result[0].Open);
    }

    [Fact]
    public async Task SelectZakenAsync_PreservesOpen_WhenTrue()
    {
        var sut = new SingleZaakSelector("zaak-001", open: true);

        var result = await sut.SelectZakenAsync("zaaktype-001");

        Assert.True(result[0].Open);
    }

    [Fact]
    public async Task SelectZakenAsync_ReturnsExactlyOneZaak_RegardlessOfDetZaaktypeId()
    {
        var sut = new SingleZaakSelector("zaak-002", open: false);

        var result = await sut.SelectZakenAsync("any-zaaktype-id");

        Assert.Single(result);
        Assert.Equal("zaak-002", result[0].FunctioneleIdentificatie);
    }
}

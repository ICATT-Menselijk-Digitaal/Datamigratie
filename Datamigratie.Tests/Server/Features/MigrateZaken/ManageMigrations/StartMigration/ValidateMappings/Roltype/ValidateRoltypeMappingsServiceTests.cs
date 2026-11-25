using Datamigratie.Common.Services.Det.Models;
using Datamigratie.Data;
using Datamigratie.Data.Entities;
using Datamigratie.Server.Constants;
using Datamigratie.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;
using Datamigratie.Tests.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.ManageMigrations.StartMigration.ValidateMappings.Roltype;

public class ValidateRoltypeMappingsServiceTests
{
    private const string DetZaaktypeId = "zaaktype-001";

    private static DatamigratieDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DatamigratieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatamigratieDbContext(options);
    }

    private static ValidateRoltypeMappingsService CreateService(DatamigratieDbContext context) =>
        new(context, NullLogger<ValidateRoltypeMappingsService>.Instance);

    private static ZaaktypeVersieMapping AddZaaktypenMapping(DatamigratieDbContext context, string detZaaktypeId = DetZaaktypeId) =>
        context.AddZaaktypenMappingWithVersie(detZaaktypeId);

    private static void AddAllRoltypeMappings(DatamigratieDbContext context, Guid zaaktypeVersieMappingId)
    {
        var mappings = MappingConstants.DetRol.Options.Select(rol => new RoltypeMapping
        {
            ZaaktypeVersieMappingId = zaaktypeVersieMappingId,
            DetRol = rol.Id,
            AlleenPdf = false,
            OzRoltypeUrl = $"https://openzaak.example.com/catalogi/api/v1/roltypen/{Guid.NewGuid()}"
        });
        context.RoltypeMappings.AddRange(mappings);
        context.SaveChanges();
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_ReturnsInvalid_WhenNoZaaktypenMappingExists()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var (isValid, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.False(isValid);
        Assert.Empty(mappings);
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_ReturnsInvalid_WhenNoRoltypeMappingsExist()
    {
        await using var context = CreateContext();
        AddZaaktypenMapping(context);
        var service = CreateService(context);

        var (isValid, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.False(isValid);
        Assert.Empty(mappings);
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_ReturnsInvalid_WhenSomeRollenAreMissing()
    {
        await using var context = CreateContext();
        var versie = AddZaaktypenMapping(context);

        // Only add mapping for one rol, leaving the other 7 missing
        context.RoltypeMappings.Add(new RoltypeMapping
        {
            ZaaktypeVersieMappingId = versie.Id,
            DetRol = "Initiator",
            AlleenPdf = false,
            OzRoltypeUrl = "https://openzaak.example.com/catalogi/api/v1/roltypen/some-uuid"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var (isValid, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.False(isValid);
        Assert.Empty(mappings);
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_ReturnsValid_WhenAllRollenAreMapped()
    {
        await using var context = CreateContext();
        var versie = AddZaaktypenMapping(context);
        AddAllRoltypeMappings(context, versie.Id);

        var service = CreateService(context);

        var (isValid, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.True(isValid);
        Assert.Equal(MappingConstants.DetRol.Options.Length, mappings.Count);
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_ReturnsMappingDictionary_WithCorrectKeys()
    {
        await using var context = CreateContext();
        var versie = AddZaaktypenMapping(context);
        AddAllRoltypeMappings(context, versie.Id);

        var service = CreateService(context);

        var (_, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        foreach (var rol in Enum.GetValues<DetRolType>())
        {
            Assert.True(mappings.ContainsKey(rol), $"Expected key '{rol}' in mappings dictionary");
        }
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_AcceptsAlleenPdf_AsValidOzRoltypeUrl()
    {
        await using var context = CreateContext();
        var versie = AddZaaktypenMapping(context);

        // Map all rollen, but one is alleen PDF (AlleenPdf = true, OzRoltypeUrl = null)
        var mappings = MappingConstants.DetRol.Options.Select((rol, i) => new RoltypeMapping
        {
            ZaaktypeVersieMappingId = versie.Id,
            DetRol = rol.Id,
            AlleenPdf = i == 0,
            OzRoltypeUrl = i == 0 ? null : $"https://openzaak.example.com/catalogi/api/v1/roltypen/{Guid.NewGuid()}"
        });
        context.RoltypeMappings.AddRange(mappings);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var (isValid, result) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.True(isValid);
        Assert.DoesNotContain(DetRolType.initiator, result.Keys);
    }

    [Fact]
    public async Task ValidateAndGetRoltypeMappings_OnlyConsidersMappingsForCorrectZaaktype()
    {
        await using var context = CreateContext();

        // Add a different zaaktype mapping with all rollen filled in
        var otherVersie = AddZaaktypenMapping(context, "other-zaaktype");
        AddAllRoltypeMappings(context, otherVersie.Id);

        // Add the target zaaktype mapping but with no rollen
        AddZaaktypenMapping(context, DetZaaktypeId);

        var service = CreateService(context);

        var (isValid, mappings) = await service.ValidateAndGetRoltypeMappings(DetZaaktypeId);

        Assert.False(isValid);
        Assert.Empty(mappings);
    }
}

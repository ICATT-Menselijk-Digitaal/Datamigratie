namespace Datamigratie.Server.Config;

public class MigrationOptions
{
    public const string SectionName = "Migration";

    public int ZaakConcurrencyLimit { get; set; } = 1;
}

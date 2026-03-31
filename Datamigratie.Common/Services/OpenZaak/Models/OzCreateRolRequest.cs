using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models;

public class OzCreateRolRequest
{
    public required Uri Zaak { get; set; }

    public required BetrokkeneType BetrokkeneType { get; set; }

    public required Uri Roltype { get; set; }

    public required OzBetrokkeneIdentificatie BetrokkeneIdentificatie { get; set; }
}

public class OzBetrokkeneIdentificatie
{
    public required string Identificatie { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter<BetrokkeneType>))]
public enum BetrokkeneType
{
    natuurlijk_persoon,
    niet_natuurlijk_persoon,
    vestiging,
    organisatorische_eenheid,
    medewerker,
}

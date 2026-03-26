using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models;

public class OzCreateRolRequest
{
    [JsonPropertyName("zaak")]
    public required Uri Zaak { get; set; }

    [JsonPropertyName("betrokkeneType")]
    public required string BetrokkeneType { get; set; }

    [JsonPropertyName("roltype")]
    public required Uri Roltype { get; set; }

    [JsonPropertyName("betrokkeneIdentificatie")]
    public required OzBetrokkeneIdentificatie BetrokkeneIdentificatie { get; set; }
}

public class OzBetrokkeneIdentificatie
{
    [JsonPropertyName("identificatie")]
    public required string Identificatie { get; set; }
}

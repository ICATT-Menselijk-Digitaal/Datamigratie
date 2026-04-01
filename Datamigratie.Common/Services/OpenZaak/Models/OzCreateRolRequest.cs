using System.Text.Json.Serialization;

namespace Datamigratie.Common.Services.OpenZaak.Models;

public class OzCreateRolRequest
{
    public Uri? Zaak { get; set; }

    public required BetrokkeneType BetrokkeneType { get; set; }

    public required Uri Roltype { get; set; }

    public required OzBetrokkeneIdentificatie BetrokkeneIdentificatie { get; set; }

    /// <summary>
    /// Set to "gemachtigde" when the betrokkene has typeBetrokkenheid == "gemachtigde".
    /// Leave empty for all other betrokkenen
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IndicatieMachtiging { get; set; }
}

public class OzBetrokkeneIdentificatie
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Identificatie { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? InpBsn { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? KvkNummer { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? VestigingsNummer { get; set; }
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

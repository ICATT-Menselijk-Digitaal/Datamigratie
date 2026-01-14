using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.DataGeneration.Zaken.Models;

public record Adres
{
    [JsonPropertyName("type")]
    public required AdresType Type { get; init; }

    [JsonPropertyName("straatnaam")]
    public string? Straatnaam { get; init; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; init; }

    [JsonPropertyName("plaatsnaam")]
    public string? Plaatsnaam { get; init; }

    [JsonPropertyName("huisletter")]
    public string? Huisletter { get; init; }

    [JsonPropertyName("huisnummer")]
    public int? Huisnummer { get; init; }

    [JsonPropertyName("huisnummertoevoeging")]
    public string? Huisnummertoevoeging { get; init; }

    [JsonPropertyName("buitenlandsAdres")]
    public required bool BuitenlandsAdres { get; init; }

    [JsonPropertyName("land")]
    public Land? Land { get; init; }
}

public record Land
{
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("naam")]
    public required string Naam { get; init; }
}

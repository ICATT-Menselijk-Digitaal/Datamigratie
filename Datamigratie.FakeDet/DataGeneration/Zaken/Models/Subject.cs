using System.Text.Json.Serialization;

namespace Datamigratie.FakeDet.DataGeneration.Zaken.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "subjecttype")]
[JsonDerivedType(typeof(Persoon), "persoon")]
[JsonDerivedType(typeof(Bedrijf), "bedrijf")]
public abstract record Subject
{
    [JsonPropertyName("telefoonnummer")]
    public string? Telefoonnummer { get; init; }

    [JsonPropertyName("telefoonnummerAlternatief")]
    public string? TelefoonnummerAlternatief { get; init; }

    [JsonPropertyName("rekeningnummer")]
    public string? Rekeningnummer { get; init; }

    [JsonPropertyName("emailadres")]
    public string? Emailadres { get; init; }

    [JsonPropertyName("ontvangenZaakNotificaties")]
    public bool? OntvangenZaakNotificaties { get; init; }

    [JsonPropertyName("toestemmingZaakNotificatiesAlleenDigitaal")]
    public bool? ToestemmingZaakNotificatiesAlleenDigitaal { get; init; }

    [JsonPropertyName("handmatigToegevoegd")]
    public required bool HandmatigToegevoegd { get; init; }

    [JsonPropertyName("adressen")]
    public IReadOnlyList<Adres>? Adressen { get; init; }
}

public record Persoon : Subject
{
    [JsonPropertyName("bsn")]
    public string? Bsn { get; init; }

    [JsonPropertyName("voornaam")]
    public string? Voornaam { get; init; }

    [JsonPropertyName("voorvoegsel")]
    public string? Voorvoegsel { get; init; }

    [JsonPropertyName("achternaam")]
    public required string Achternaam { get; init; }

    [JsonPropertyName("geslacht")]
    public Geslacht? Geslacht { get; init; }

    [JsonPropertyName("geboortedatum")]
    public DateOnly? Geboortedatum { get; init; }
}

public record Bedrijf : Subject
{
    [JsonPropertyName("kvkNummer")]
    public string? KvkNummer { get; init; }

    [JsonPropertyName("vestigingsnummer")]
    public string? Vestigingsnummer { get; init; }

    [JsonPropertyName("bedrijfsnaam")]
    public required string Bedrijfsnaam { get; init; }

    [JsonPropertyName("rechtsvorm")]
    public string? Rechtsvorm { get; init; }
}

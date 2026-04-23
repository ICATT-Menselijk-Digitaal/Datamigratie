using System.Text.Json;
using System.Text.Json.Serialization;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Common.Converters;

/// <summary>
/// This converter is necessary because there's no attribute or option to make [JsonPolymorphic] read the discriminator out-of-order.
/// The serializer reads properties in a single forward pass and requires the discriminator first. This is not the case in DET
/// </summary>
public class DetZaakdataConverter : JsonConverter<DetZaakdata>
{
    public override DetZaakdata? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("type", out var discriminator))
            throw new JsonException("Missing 'type' discriminator property on DetZaakdata.");

        var type = discriminator.GetString();

        return type switch
        {
            "string" => root.Deserialize<DetStringDataElement>(options),
            "boolean" => root.Deserialize<DetBooleanDataElement>(options),
            "calendar" => root.Deserialize<DetCalendarDataElement>(options),
            "datummettijdstip" => root.Deserialize<DetDatumMetTijdstipDataElement>(options),
            "decimaal" => root.Deserialize<DetDecimaalDataElement>(options),
            "decimalen" => root.Deserialize<DetDecimalenDataElement>(options),
            "optie" => root.Deserialize<DetOptieDataElement>(options),
            "adresgegevens" => root.Deserialize<DetAdresgegevensDataElement>(options),
            "referentietabel_record" => root.Deserialize<DetReferentietabelRecordDataElement>(options),
            "afstand" => root.Deserialize<DetAfstandDataElement>(options),
            "generieke_afspraak" => root.Deserialize<DetGeneriekeAfspraakDataElement>(options),
            "geo_informatie" => root.Deserialize<DetGeoInformatieDataElement>(options),
            "digitale_notificaties" => root.Deserialize<DetDigitaleNotificatiesDataElement>(options),
            "zaak_besluit" => root.Deserialize<DetZaakBesluitDataElement>(options),
            "opties" => root.Deserialize<DetOptiesDataElement>(options),
            "strings" => root.Deserialize<DetStringsDataElement>(options),
            "zaak_documenten" => root.Deserialize<DetDocumentDataElement>(options),
            "select_documents" => root.Deserialize<DetSelectDocumentDataElement>(options),
            "aanvullijst" => root.Deserialize<DetAanvullijstDataElement>(options),
            _ => throw new JsonException($"Unknown zaakdata type '{type}'.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DetZaakdata value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

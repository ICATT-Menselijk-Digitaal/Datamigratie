using System.Text.Json;
using System.Text.Json.Serialization;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Common.Converters;

/// <summary>
/// This converter is necessary because there's no attribute or option to make [JsonPolymorphic] read the discriminator out-of-order.
/// The serializer reads properties in a single forward pass and requires the discriminator first. This is not the case in DET
/// </summary>
public class DetSubjectConverter : JsonConverter<DetSubject>
{
    public override DetSubject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("subjecttype", out var discriminator))
            throw new JsonException("Missing 'subjecttype' discriminator property on DetSubject.");

        var subjectType = discriminator.GetString();

        return subjectType switch
        {
            "persoon" => root.Deserialize<DetPersoon>(options),
            "bedrijf" => root.Deserialize<DetBedrijf>(options),
            _ => throw new JsonException($"Unknown subjecttype '{subjectType}'.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DetSubject value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Datamigratie.Common.Converters
{
    /// <summary>
    /// A generic JSON converter for enums that treats a "None" enum value as an empty string.
    /// All other enum values are serialized/deserialized using their name.
    /// </summary>
    public class JsonStringEnumWithNoneConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                if (Enum.TryParse<TEnum>("None", out var noneValue))
                {
                    return noneValue;
                }
                throw new JsonException($"Enum {typeof(TEnum).Name} does not have a 'None' value to map empty string to.");
            }

            if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }

            throw new JsonException($"Unknown {typeof(TEnum).Name} value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var name = value.ToString();
            writer.WriteStringValue(name == "None" ? "" : name);
        }
    }

    /// <summary>
    /// Factory to create JsonStringEnumWithNoneConverter for any enum type.
    /// Use this on enum declarations: [JsonConverter(typeof(JsonStringEnumWithNoneConverter))]
    /// </summary>
    public class JsonStringEnumWithNoneConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(JsonStringEnumWithNoneConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}

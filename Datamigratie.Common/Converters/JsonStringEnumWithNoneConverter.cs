using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Datamigratie.Common.Converters
{
    /// <summary>
    /// A generic JSON converter for nullable enums that treats null as an empty string.
    /// All other enum values are serialized/deserialized using their name.
    /// </summary>
    public class JsonStringEnumWithNullConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
    {
        public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }

            throw new JsonException($"Unknown {typeof(TEnum).Name} value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteStringValue("");
            }
            else
            {
                writer.WriteStringValue(value.Value.ToString());
            }
        }
    }
}

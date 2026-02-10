using System.Text.Json;
using System.Text.Json.Serialization;

namespace Datamigratie.Common.Converters
{
    /// <summary>
    /// A generic JSON converter for enums that have a 'None' value.
    /// 'None' is serialized as an empty string, and empty strings are deserialized as 'None'.
    /// All other enum values are serialized/deserialized using their name.
    /// </summary>
    public class JsonStringEnumWithBlankConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        private const string NoneValue = "Blank";

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (value == "")
            {
                return Enum.Parse<TEnum>(NoneValue);
            }

            if (Enum.TryParse<TEnum>(value, out var result))
            {
                return result;
            }

            throw new JsonException($"Unknown {typeof(TEnum).Name} value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var stringValue = value.ToString();
            if (stringValue == NoneValue)
            {
                writer.WriteStringValue("");
            }
            else
            {
                writer.WriteStringValue(stringValue);
            }
        }
    }
}

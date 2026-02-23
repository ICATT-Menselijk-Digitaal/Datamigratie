using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Datamigratie.Common.Converters;

public class DetDateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (string.IsNullOrEmpty(dateString))
        {
            throw new JsonException("Date string was null or empty");
        }

        var dateStringSpan = dateString.AsSpan();
        var zoneIndex = dateStringSpan.IndexOf('[');
        var dateTimeWithoutNamedTimezone = zoneIndex >= 0 ? dateStringSpan.Slice(0, zoneIndex) : dateStringSpan;

        if (DateOnly.TryParse(dateTimeWithoutNamedTimezone, CultureInfo.InvariantCulture, out var dateOnly))
        {
            return dateOnly;
        }

        if (DateTime.TryParse(dateTimeWithoutNamedTimezone, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        if (DateTimeOffset.TryParse(dateTimeWithoutNamedTimezone, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeOffset))
        {
            return DateOnly.FromDateTime(dateTimeOffset.DateTime);
        }

        throw new JsonException($"Unable to parse '{dateString}' as DateOnly");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}

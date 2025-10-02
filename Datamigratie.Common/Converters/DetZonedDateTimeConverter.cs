﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Datamigratie.Common.Converters
{

    public class DetZonedDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Read the raw string
            var dateString = reader.GetString();

            if (dateString == null)
            {
                throw new Exception("Error while trying to deserialize a DET datetime zoned. JSON String was null"); 
            }

            var zoneIndex = dateString.IndexOf('[');
            var dateTimeWithoutNamedTimezone = zoneIndex >= 0 ? dateString.Substring(0, zoneIndex) : dateString;

            return DateTime.Parse(dateTimeWithoutNamedTimezone);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new Exception("Serialization of DET zoned date time is not implemented");
        }
    }
}

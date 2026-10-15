using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GFI_Upgrated.UI.Helpers;
public class UtcToLocalDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawString = reader.GetString();
        if (string.IsNullOrWhiteSpace(rawString)) return default;

        var dateTime = reader.GetDateTime();

        // 1. Pure calendar date (midnight 00:00:00 without explicit 'Z' UTC flag)
        // Keep exact date as-is (do NOT shift days)
        if (dateTime.TimeOfDay == TimeSpan.Zero && !rawString.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

        // 2. Real UTC timestamp with time component (CreatedDate, UpdatedDate)
        if (dateTime.Kind == DateTimeKind.Unspecified || dateTime.Kind == DateTimeKind.Utc)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
        }

        return dateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            // Calendar date: write as ISO date string without UTC shifting
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
        else
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }
}
//public class UtcToLocalDateTimeConverter : JsonConverter<DateTime>
//{
//    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var dateTime = reader.GetDateTime();

//        if (dateTime.Kind == DateTimeKind.Unspecified || dateTime.Kind == DateTimeKind.Utc)
//        {
//            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc).ToLocalTime();
//        }

//        return dateTime;
//    }

//    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
//    }
//}

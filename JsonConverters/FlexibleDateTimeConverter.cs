using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IEEE.JsonConverters
{
    /// <summary>
    /// Flexible converter for non-nullable DateTime.
    /// Tries ISO/round-trip parsing first, then falls back to common local formats.
    /// </summary>
    public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] SupportedFormats =
        {
            // US-style formats
            "M/d/yyyy h:mm:ss tt",
            "M/d/yyyy h:mm tt",
            "M/d/yyyy H:mm:ss",
            "M/d/yyyy H:mm",
            "M/d/yyyy",

            // European-style formats
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm",
            "dd/MM/yyyy",

            // ISO-like formats
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
            "yyyy-MM-ddTHH:mm:ssK",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token parsing DateTime. Expected String, got {reader.TokenType}.");
            }

            var raw = reader.GetString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                // For non-nullable DateTime, empty/whitespace is not allowed.
                throw new JsonException("Cannot convert empty or whitespace string to non-nullable DateTime.");
            }

            var text = raw.Trim();

            if (TryParseFlexible(text, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse DateTime value: '{raw}'.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Always serialize in ISO 8601 round-trip format for consistency.
            writer.WriteStringValue(value.ToString("O", CultureInfo.InvariantCulture));
        }

        internal static bool TryParseFlexible(string input, out DateTime value)
        {
            // First try standard/ISO parsing with invariant culture.
            if (DateTime.TryParse(
                    input,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out value))
            {
                return true;
            }

            // Fall back to a curated list of common local/ISO-like formats.
            if (DateTime.TryParseExact(
                    input,
                    SupportedFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out value))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Flexible converter for nullable DateTime.
    /// Treats empty or whitespace strings as null, otherwise uses the same fallback parsing logic.
    /// </summary>
    public sealed class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Unexpected token parsing DateTime?. Expected String or Null, got {reader.TokenType}.");
            }

            var raw = reader.GetString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                // Empty/whitespace is treated as null instead of failing.
                return null;
            }

            var text = raw.Trim();

            if (FlexibleDateTimeConverter.TryParseFlexible(text, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse DateTime? value: '{raw}'.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                // Use the same ISO 8601 round-trip format as the non-nullable converter.
                writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}


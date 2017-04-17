
namespace Sparkle.Services.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class SimpleDateTimeConverter : DateTimeConverterBase
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        private DateTimeStyles _dateTimeStyles = DateTimeStyles.AssumeUniversal;
        private string _dateTimeFormat = DefaultDateTimeFormat;
        private CultureInfo _culture = CultureInfo.InvariantCulture;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool nullable = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
            Type t = nullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "Cannot convert null value to {0}.", objectType);
                    message = FormatExceptionMessage((IJsonLineInfo)reader, reader.Path, message);
                    throw new JsonSerializationException(message);
                }

                return null;
            }
            else
            {
                if (reader.TokenType == JsonToken.Date)
                {
                    if (t == typeof(DateTimeOffset))
                    {
                        return new DateTimeOffset((DateTime)reader.Value);
                    }

                    return reader.Value;
                }
                else
                {
                    if (reader.TokenType != JsonToken.String)
                    {
                        var message = string.Format(CultureInfo.InvariantCulture, string.Format(CultureInfo.InvariantCulture, "Unexpected token parsing date. Expected String, got {0}.", reader.TokenType));
                        message = FormatExceptionMessage((IJsonLineInfo)reader, reader.Path, message);
                        throw new JsonSerializationException(message);
                    }

                    string dateText = reader.Value.ToString();
                    if (string.IsNullOrEmpty(dateText) && nullable)
                    {
                        return null;
                    }

                    if (t == typeof(DateTimeOffset))
                    {
                        if (!string.IsNullOrEmpty(this._dateTimeFormat))
                        {
                            return DateTimeOffset.ParseExact(dateText, this._dateTimeFormat, this._culture, this._dateTimeStyles);
                        }

                        return DateTimeOffset.Parse(dateText, this._culture, this._dateTimeStyles);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this._dateTimeFormat))
                        {
                            return DateTime.ParseExact(dateText, this._dateTimeFormat, this._culture, this._dateTimeStyles);
                        }

                        return DateTime.Parse(dateText, this._culture, this._dateTimeStyles);
                    }
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                text = dateTime.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this._culture);
            }
            else
            {
                if (!(value is DateTimeOffset))
                {
                    throw new JsonSerializationException(string.Format(CultureInfo.InvariantCulture, "Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.", value != null ? value.GetType() : null));
                }

                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if ((this._dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal || (this._dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();
                }

                text = dateTimeOffset.ToString(this._dateTimeFormat ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", this._culture);
            }

            writer.WriteValue(text);
        }

        internal static string FormatExceptionMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            if (!message.EndsWith(Environment.NewLine))
            {
                message = message.Trim();
                if (!message.EndsWith("."))
                {
                    message += ".";
                }

                message += " ";
            }

            message += string.Format(CultureInfo.InvariantCulture, "Path '{0}'", path);
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                message += string.Format(CultureInfo.InvariantCulture, ", line {0}, position {1}", lineInfo.LineNumber, lineInfo.LinePosition);
            }

            message += ".";
            return message;
        }
    }
}

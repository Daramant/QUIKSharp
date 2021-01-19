using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.Serialization.Json.Converters
{
    /// <summary>
    /// Serialize Decimal to string without trailing zeros
    /// </summary>
    public class DecimalG29ToStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(decimal));
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            return decimal.Parse((string)reader.Value, serializer.Culture);
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(((decimal)value).ToString("G29", serializer.Culture));
        }
    }
}

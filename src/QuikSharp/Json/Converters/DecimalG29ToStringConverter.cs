using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Converters
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
            var t = JToken.Load(reader);
            return t.Value<decimal>();
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            decimal d = (decimal)value;
            var t = JToken.FromObject(d.ToString("G29"));
            t.WriteTo(writer);
        }
    }
}

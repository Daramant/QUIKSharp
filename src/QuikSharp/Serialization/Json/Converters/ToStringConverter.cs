using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Serialization.Json.Converters
{
    /// <summary>
    /// Serialize as string with ToString()
    /// </summary>
    public class ToStringConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var underType = Nullable.GetUnderlyingType(objectType);
            var baseType = underType ?? objectType;
            return Convert.ChangeType(reader.Value, baseType, serializer.Culture);
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToString(value, serializer.Culture));
        }
    }
}

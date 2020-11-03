using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Converters
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
            var t = JToken.Load(reader);
            T target = t.Value<T>();
            return target;
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var t = JToken.FromObject(value.ToString());
            t.WriteTo(writer);
        }
    }
}

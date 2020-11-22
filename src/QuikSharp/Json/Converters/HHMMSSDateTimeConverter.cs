using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Converters
{
    /// <summary>
    /// Convert DateTime to HHMMSS
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class HHMMSSDateTimeConverter : JsonConverter
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
            var target = t.Value<string>();
            if (target == null) return null;
            var hh = int.Parse(target.Substring(0, 2));
            var mm = int.Parse(target.Substring(2, 2));
            var ss = int.Parse(target.Substring(4, 2));
            var now = DateTime.Now;
            var dt = new DateTime(now.Year, now.Month, now.Day, hh, mm, ss);
            return dt;
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("HHmmss"));
        }
    }
}

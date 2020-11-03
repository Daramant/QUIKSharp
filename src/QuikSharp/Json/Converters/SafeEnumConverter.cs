using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Converters
{
    /// <summary>
    /// Limits enum serialization only to defined values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeEnumConverter<T> : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var isDef = Enum.IsDefined(typeof(T), value);
            if (!isDef)
            {
                value = null;
            }

            base.WriteJson(writer, value, serializer);
        }
    }
}

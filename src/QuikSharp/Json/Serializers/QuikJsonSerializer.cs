using Newtonsoft.Json;
using QuikSharp.Json.Converters;
using QuikSharp.Json.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace QuikSharp.Json.Serializers
{
    public class QuikJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializer _serializer;

        [ThreadStatic]
        private static StringBuilder _stringBuilder;

        public QuikJsonSerializer()
        {
            _serializer = new JsonSerializer
            {
                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };

            //_serializer.Converters.Add(new MessageConverter(quikClient));
        }

        public void AddConverter(JsonConverter jsonConverter) // TODO: Удалить.
        {
            _serializer.Converters.Add(jsonConverter);
        }

        /// <inheritdoc />
        public T Deserialize<T>(string json)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize<T>(reader);
                return value;
            }
        }

        /// <inheritdoc />
        public object Deserialize(string json, Type type)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize(reader, type);
                return value;
            }
        }

        /// <inheritdoc />
        public string Serialize<T>(T obj)
        {
            if (_stringBuilder == null)
            {
                _stringBuilder = new StringBuilder();
            }

            using (var writer = new JsonTextWriter(new StringWriter(_stringBuilder)))
            {
                try
                {
                    // reader will get buffer from array pool
                    writer.ArrayPool = JsonArrayPool.Instance;
                    _serializer.Serialize(writer, obj);
                    return _stringBuilder.ToString();
                }
                finally
                {
                    _stringBuilder.Clear();
                }
            }
        }
    }
}

// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace QuikSharp
{
    /// <summary>
    /// Extensions for JSON.NET
    /// </summary>
    public static class JsonExtensions
    {
        private static JsonSerializer _serializer;

        [ThreadStatic]
        private static StringBuilder _stringBuilder;

        static JsonExtensions()
        {
            _serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        ///
        /// </summary>
        public static T FromJson<T>(this string json)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize<T>(reader);
                return value;
            }
        }

        internal static IMessage FromJson(this string json, QuikService service)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = service.Serializer.Deserialize<IMessage>(reader);
                return value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static object FromJson(this string json, Type type)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize(reader, type);
                return value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static string ToJson<T>(this T obj)
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

        /// <summary>
        /// Returns indented JSON
        /// </summary>
        public static string ToJsonFormatted<T>(this T obj)
        {
            var message = JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None, // Objects
                    Formatting = Formatting.Indented,
                    // NB this is important for correctness and performance
                    // Transaction could have many null properties
                    NullValueHandling = NullValueHandling.Ignore
                });
            return message;
        }
    }
}
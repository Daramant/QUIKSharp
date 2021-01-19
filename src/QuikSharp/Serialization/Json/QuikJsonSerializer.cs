using Newtonsoft.Json;
using QuikSharp.Messages;
using QuikSharp.QuikEvents;
using QuikSharp.Serialization.Exceptions;
using QuikSharp.Serialization.Json.Converters;
using QuikSharp.Serialization.Json.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace QuikSharp.Serialization.Json
{
    public class QuikJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer;
        private readonly IResultTypeProvider _resultTypeProvider;
        private readonly IEventTypeProvider _eventTypeProvider;

        [ThreadStatic]
        private static StringBuilder _stringBuilder;

        public QuikJsonSerializer(
            IResultTypeProvider resultTypeProvider,
            IEventTypeProvider eventTypeProvider)
        {
            _resultTypeProvider = resultTypeProvider;
            _eventTypeProvider = eventTypeProvider;

            _serializer = new JsonSerializer
            {
                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public Envelope<ResultHeader, IResult> DeserializeResultEnvelope(string data)
        {
            using (var stringReader = new StringReader(data))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                jsonReader.SupportMultipleContent = true;
                // reader will get buffer from array pool
                jsonReader.ArrayPool = JsonArrayPool.Instance;

                if (!jsonReader.Read())
                    throw new QuikSerializationException($"Отсутствует заголовок конверта. data: '{data}'.");

                var header = _serializer.Deserialize<ResultHeader>(jsonReader);

                if (!jsonReader.Read())
                    throw new QuikSerializationException($"Отсутствуют данные конверта. data: '{data}'.");

                switch (header.Status)
                {
                    case ResultStatus.Ok:
                        {
                            if (!_resultTypeProvider.TryGetResultType(header.CommandId, out var resultType))
                                throw new QuikSerializationException($"Не удалось получить тип результата для команды с идентификатором: {header.CommandId}.");

                            var result = _serializer.Deserialize(jsonReader, resultType);

                            return new Envelope<ResultHeader, IResult>(header, (IResult)result);
                        }

                    case ResultStatus.Error:
                        {
                            var result = _serializer.Deserialize(jsonReader, typeof(Result<string>));

                            return new Envelope<ResultHeader, IResult>(header, (IResult)result);
                        }

                    default:
                        throw new QuikSerializationException($"Статус: '{header.Status}' не поддерживается.");
                }
            }
        }

        public Envelope<EventHeader, IEvent> DeserializeEventEnvelope(string data)
        {
            using (var stringReader = new StringReader(data))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                jsonReader.SupportMultipleContent = true;
                // reader will get buffer from array pool
                jsonReader.ArrayPool = JsonArrayPool.Instance;

                if (!jsonReader.Read())
                    throw new QuikSerializationException($"Отсутствует заголовок конверта. data: '{data}'.");

                var header = _serializer.Deserialize<EventHeader>(jsonReader);

                if (!jsonReader.Read())
                    throw new QuikSerializationException($"Отсутствуют данные конверта. data: '{data}'.");

                if (!_eventTypeProvider.TryGetEventType(header.EventName, out var eventType))
                    throw new QuikSerializationException($"Не найден тип события для десериализации по идентификатору: {header.EventName}.");

                var @event = _serializer.Deserialize(jsonReader, eventType);

                return new Envelope<EventHeader, IEvent>(header, (IEvent)@event);
            }
        }

        /// <inheritdoc />
        public T Deserialize<T>(string data)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize<T>(reader);
                return value;
            }
        }

        /// <inheritdoc />
        public object Deserialize(string data, Type type)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                var value = _serializer.Deserialize(reader, type);
                return value;
            }
        }

        /// <inheritdoc />
        public object Deserialize(StringReader stringReader, Type type)
        {
            using (var reader = new JsonTextReader(stringReader))
            {
                reader.SupportMultipleContent = true;
                reader.CloseInput = false;
                if (!reader.Read())
                    return null;

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

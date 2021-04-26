using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using QuikSharp.Messages;
using QuikSharp.QuikEvents;
using QuikSharp.Serialization.Exceptions;
using QuikSharp.Serialization.Json.Tools;
using System;
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

        private readonly ObjectPool<StringBuilder> _stringBuilderPool = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());


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

        /// <inheritdoc />
        public Envelope<ResultHeader, IResult> DeserializeResultEnvelope(string data)
        {
            using (var jsonReader = new JsonTextReader(new StringReader(data)))
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

        /// <inheritdoc />
        public Envelope<EventHeader, IEvent> DeserializeEventEnvelope(string data)
        {
            using (var jsonReader = new JsonTextReader(new StringReader(data)))
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
                    throw new QuikSerializationException($"Не найден тип события для десериализации по событию: '{header.EventName}'.");

                var @event = (IEvent)_serializer.Deserialize(jsonReader, eventType);
                @event.Name = header.EventName;

                return new Envelope<EventHeader, IEvent>(header, @event);
            }
        }

        /// <inheritdoc />
        public T Deserialize<T>(string data)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                
                return _serializer.Deserialize<T>(reader);
            }
        }

        /// <inheritdoc />
        public object Deserialize(string data, Type type)
        {
            using (var reader = new JsonTextReader(new StringReader(data)))
            {
                // reader will get buffer from array pool
                reader.ArrayPool = JsonArrayPool.Instance;
                
                return _serializer.Deserialize(reader, type);
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
            var stringBuilder = _stringBuilderPool.Get();

            try
            {
                using (var writer = new JsonTextWriter(new StringWriter(stringBuilder)))
                {
                    // reader will get buffer from array pool
                    writer.ArrayPool = JsonArrayPool.Instance;

                    _serializer.Serialize(writer, obj);
                    return stringBuilder.ToString();
                }
            }
            finally
            {
                _stringBuilderPool.Return(stringBuilder);
            }
        }
    }
}

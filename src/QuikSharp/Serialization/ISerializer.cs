using Newtonsoft.Json;
using QuikSharp.Messages;
using QuikSharp.Serialization.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuikSharp.Serialization
{
    public interface ISerializer
    {
        /// <summary>
        ///
        /// </summary>
        Envelope<ResultHeader, IResult> DeserializeResultEnvelope(string data);

        /// <summary>
        ///
        /// </summary>
        Envelope<EventHeader, IEvent> DeserializeEventEnvelope(string data);

        /// <summary>
        ///
        /// </summary>
        T Deserialize<T>(string data);

        /// <summary>
        ///
        /// </summary>
        object Deserialize(string data, Type type);

        /// <summary>
        ///
        /// </summary>
        object Deserialize(StringReader stringReader, Type type);

        /// <summary>
        ///
        /// </summary>
        string Serialize<T>(T obj);
    }
}

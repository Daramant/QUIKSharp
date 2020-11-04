using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Json.Serializers
{
    public interface IJsonSerializer
    {
        /// <summary>
        ///
        /// </summary>
        T Deserialize<T>(string json);

        /// <summary>
        ///
        /// </summary>
        object Deserialize(string json, Type type);

        /// <summary>
        ///
        /// </summary>
        string Serialize<T>(T obj);
    }
}

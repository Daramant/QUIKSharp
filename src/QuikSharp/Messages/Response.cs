using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    internal class Response<T>: Message<T>, IResponse<T>
    {
        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        public Response()
            : base()
        {
        }

        public Response(T data)
            : base(data)
        {
        }
    }
}

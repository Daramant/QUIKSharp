using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    internal class Result<T>: Message<T>, IResult<T>
    {
        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        public Result()
            : base()
        {
        }

        public Result(T data)
            : base(data)
        {
        }
    }
}

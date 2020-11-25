using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    internal class Command<T>: Message<T>, ICommand<T>
    {
        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }

        /// <summary>
        /// Some messages are valid only for a short time, e.g. buy/sell orders
        /// </summary>
        [JsonProperty(PropertyName = "v")]
        public DateTime? ValidUntil { get; set; }

        public Command()
            : base()
        {
        }

        public Command(T data, string name, DateTime? validUntil = null)
            : base(data)
        {
            Name = name;
            ValidUntil = validUntil;
        }
    }
}

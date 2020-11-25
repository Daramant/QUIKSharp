using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    internal class Event<T>: Message<T>, IEvent<T>
    {
        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        [JsonProperty(PropertyName = "n")]
        public EventName Name { get; set; }

        public Event()
            : base()
        {
        }

        public Event(T data, EventName name)
            : base(data)
        {
            Name = name;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public class Event<T>: Message<T>, IEvent<T>
    {
        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        [JsonIgnore]
        public EventName Name { get; set; }

        public Event()
            : base()
        {
        }
    }
}

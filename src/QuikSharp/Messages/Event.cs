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
        [JsonProperty(PropertyName = "cmd")] // TODO: Переименовать в name (или n).
        public string Name { get; set; }

        public Event()
            : base()
        {
        }

        public Event(T data, string name)
            : base(data)
        {
            Name = name;
        }
    }
}

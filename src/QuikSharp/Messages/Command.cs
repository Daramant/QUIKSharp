using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public class Command<T>: Message<T>, ICommand<T>
    {
        /// <inheritdoc/>
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public Command()
            : base()
        {
        }

        public Command(string name, T data)
            : base(data)
        {
            Name = name;
        }
    }
}

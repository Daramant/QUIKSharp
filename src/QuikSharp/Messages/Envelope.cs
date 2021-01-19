using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct Envelope<THeader, TBody> where TBody : IMessage
    {
        [JsonProperty(PropertyName = "h")]
        public THeader Header { get; }

        [JsonProperty(PropertyName = "b")]
        public TBody Body { get; }

        public Envelope(THeader header, TBody body)
        {
            Header = header;
            Body = body;
        }
    }
}

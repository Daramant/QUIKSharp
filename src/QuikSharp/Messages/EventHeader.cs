using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct EventHeader
    {
        [JsonProperty(PropertyName = "en")]
        public EventName EventName { get; set; }
    }
}

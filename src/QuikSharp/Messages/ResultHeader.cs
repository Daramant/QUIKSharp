using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct ResultHeader
    {
        [JsonProperty(PropertyName = "cid")]
        public long CommandId { get; set; }

        [JsonProperty(PropertyName = "status")]
        public ResultStatus Status { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct CommandHeader
    {
        [JsonProperty(PropertyName = "cid")]
        public long CommandId { get; set; }

        public CommandHeader(long commandId)
        {
            CommandId = commandId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct CommandHeader
    {
        public long CommandId { get; set; }

        public CommandHeader(long commandId)
        {
            CommandId = commandId;
        }
    }
}

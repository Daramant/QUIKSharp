using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct ResultHeader
    {
        public long CommandId { get; set; }

        public ResultStatus Status { get; set; }
    }
}

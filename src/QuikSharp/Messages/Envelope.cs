using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public struct Envelope<THeader, TData> where TData : IMessage
    {
        public THeader Header { get; }

        public TData Data { get; }

        public Envelope(THeader header, TData data)
        {
            Header = header;
            Data = data;
        }
    }
}

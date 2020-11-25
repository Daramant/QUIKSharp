using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QuikSharp.QuikClient
{
    public class QuikClientOptions
    {
        public IPAddress Host { get; set; }

        public int CommandPort { get; set; }

        public int EventPort { get; set; }

        public TimeSpan SendCommandTimeout { get; set; }

        public static QuikClientOptions GetDefault()
        {
            return new QuikClientOptions
            {
                Host = IPAddress.Parse("127.0.0.1"),
                CommandPort = 34130,
                EventPort = 34131,
                SendCommandTimeout = TimeSpan.Zero
            };
        }
    }
}

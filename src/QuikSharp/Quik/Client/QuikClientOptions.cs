using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QuikSharp.Quik.Client
{
    public class QuikClientOptions
    {
        public IPAddress Host { get; set; }

        public int CommandPort { get; set; }

        public int EventPort { get; set; }

        public Encoding Encoding { get; set; }

        public TimeSpan ConnectTimeout { get; set; }

        public int ConnectAttemptCount { get; set; }

        public TimeSpan SendCommandTimeout { get; set; }

        public TimeSpan StopTimeout { get; set; }

        public static QuikClientOptions GetDefault()
        {
            return new QuikClientOptions
            {
                Host = IPAddress.Parse("127.0.0.1"),
                CommandPort = 34130,
                EventPort = 34131,
                Encoding = Encoding.GetEncoding(1251),
                ConnectTimeout = TimeSpan.FromMilliseconds(100),
                ConnectAttemptCount = 0,
                SendCommandTimeout = TimeSpan.Zero,
                StopTimeout = TimeSpan.FromSeconds(5),
            };
        }
    }
}

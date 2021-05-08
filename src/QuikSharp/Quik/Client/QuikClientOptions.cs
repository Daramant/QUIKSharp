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

        public TimeSpan ConnectionAttemptTimeout { get; set; }

        public int ConnectionAttemptCount { get; set; }

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
                ConnectionAttemptTimeout = TimeSpan.FromMilliseconds(100),
                ConnectionAttemptCount = 600,
                SendCommandTimeout = TimeSpan.FromMinutes(1),
                StopTimeout = TimeSpan.FromSeconds(5),
            };
        }
    }
}

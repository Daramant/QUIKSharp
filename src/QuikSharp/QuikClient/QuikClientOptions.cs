using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikClient
{
    public class QuikClientOptions
    {
        public string Host { get; set; }

        public int ResponsePort { get; set; }

        public int CallbackPort { get; set; }

        public static QuikClientOptions GetDefault()
        {
            return new QuikClientOptions
            {
                Host = "127.0.0.1",
                ResponsePort = 34130,
                CallbackPort = 34131
            };
        }
    }
}

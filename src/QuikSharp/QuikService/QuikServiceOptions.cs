using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikService
{
    public class QuikServiceOptions
    {
        public string Host { get; set; }

        public int ResponsePort { get; set; }

        public int CallbackPort { get; set; }

        public static QuikServiceOptions GetDefault()
        {
            return new QuikServiceOptions
            {
                Host = "127.0.0.1",
                ResponsePort = 34130,
                CallbackPort = 34131
            };
        }
    }
}

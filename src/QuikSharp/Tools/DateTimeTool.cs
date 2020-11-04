using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Tools
{
    public static class DateTimeTool
    {
        private static readonly long Epoch = (new DateTime(1970, 1, 1, 3, 0, 0, 0)).Ticks / 10000L;

        public static long GetCurrentTime()
        {
            return DateTime.Now.Ticks / 10000L - Epoch;
        }
    }
}

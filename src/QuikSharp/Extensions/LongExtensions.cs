using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.Extensions
{
    public static class LongExtensions
    {
        public static string ToQuikString(this long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

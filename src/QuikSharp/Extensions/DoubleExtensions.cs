using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToQuikString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

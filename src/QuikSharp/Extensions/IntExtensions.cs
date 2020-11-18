using QuikSharp.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.Extensions
{
    public static class IntExtensions
    {
        public static string ToQuikStringLookup(this int value)
        {
            return IntToStringTool.GetString(value) ?? value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

using QuikSharp.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Extensions
{
    public static class EnumExtensions
    {
        public static string ToQuikIntStringLookup(this Enum value)
        {
            return (Convert.ToInt32(value)).ToQuikStringLookup();
        }
    }
}

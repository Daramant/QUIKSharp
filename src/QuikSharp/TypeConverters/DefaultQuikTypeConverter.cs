using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.TypeConverters
{
    public class DefaultQuikTypeConverter : ITypeConverter
    {
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        public string ToString(int value)
        {
            return value.ToString(_cultureInfo);
        }

        public string ToStringLookup(int value)
        {
            return value.ToString(_cultureInfo);
        }

        public string ToString(long value)
        {
            return value.ToString(_cultureInfo);
        }

        public string ToString(decimal value)
        {
            return value.ToString(_cultureInfo);
        }

        public string ToString<TEnum>(TEnum value)
            where TEnum : Enum
        {
            return value.ToString();
        }

        public bool IsEnumDefined<TEnum>(TEnum value)
            where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
    }
}

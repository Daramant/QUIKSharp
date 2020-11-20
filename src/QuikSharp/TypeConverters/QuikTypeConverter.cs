using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.TypeConverters
{
    public class QuikTypeConverter : ITypeConverter
    {
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        private readonly ConcurrentDictionary<int, string> _intToStringDictionary = new ConcurrentDictionary<int, string>();
        private readonly ConcurrentDictionary<Enum, string> _enumToStringDictionary = new ConcurrentDictionary<Enum, string>();

        public QuikTypeConverter()
        {

        }

        public string ToString(int value)
        {
            return value.ToString(_cultureInfo);
        }

        public string ToStringLookup(int value)
        {
            if (_intToStringDictionary.TryGetValue(value, out var stringValue))
            {
                return stringValue;
            }
            else
            {
                stringValue = ToString(value);
                _intToStringDictionary[value] = stringValue;
                return stringValue;
            }
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
            if (_enumToStringDictionary.TryGetValue(value, out var stringValue))
            {
                return stringValue;
            }
            else
            {
                stringValue = value.ToString();
                _enumToStringDictionary[value] = stringValue;
                return stringValue;
            }
        }
    }
}

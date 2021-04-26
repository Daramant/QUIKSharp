using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.TypeConverters
{
    public class CachingQuikTypeConverter : ITypeConverter
    {
        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        private readonly ConcurrentDictionary<int, string> _intToStringDictionary = new ConcurrentDictionary<int, string>();
        private readonly ConcurrentDictionary<Type, EnumData> _enumsDictionary = new ConcurrentDictionary<Type, EnumData>();

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
            var enumData = GetOrCreateEnumData(typeof(TEnum));
            return enumData.EnumToStringDictionary[value];
        }

        public bool IsEnumDefined<TEnum>(TEnum value)
            where TEnum : Enum
        {
            var enumData = GetOrCreateEnumData(typeof(TEnum));
            return enumData.Values.Contains(value);
        }

        public TEnum ParseEnum<TEnum>(string value)
            where TEnum : Enum
        {
            var enumData = GetOrCreateEnumData(typeof(TEnum));
            return (TEnum)enumData.StringToEnumDictionary[value];
        }

        public bool TryParseEnum<TEnum>(string stringValue, out TEnum enumValue)
            where TEnum : struct, Enum
        {
            var enumData = GetOrCreateEnumData(typeof(TEnum));
            var hasValue = enumData.StringToEnumDictionary.TryGetValue(stringValue, out var typedEnumValue);
            enumValue = (TEnum)typedEnumValue;
            return hasValue;
        }

        private EnumData GetOrCreateEnumData(Type enumType)
        {
            if (_enumsDictionary.TryGetValue(enumType, out var enumData))
                return enumData;
            
            lock (_enumsDictionary)
            {
                if (_enumsDictionary.TryGetValue(enumType, out enumData))
                    return enumData;
                
                enumData = new EnumData();

                foreach (Enum @enum in Enum.GetValues(enumType))
                {
                    var enumStringValue = @enum.ToString();
                    enumData.Values.Add(@enum);
                    enumData.EnumToStringDictionary[@enum] = enumStringValue;
                    enumData.StringToEnumDictionary[enumStringValue] = @enum;
                };

                _enumsDictionary[enumType] = enumData;
                return enumData;
            }
        }
    }
}

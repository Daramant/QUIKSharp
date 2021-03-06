﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.TypeConverters
{
    public interface ITypeConverter
    {
        string ToString(int value);

        string ToStringLookup(int value);

        string ToString(long value);

        string ToString(decimal value);

        string ToString<TEnum>(TEnum value)
            where TEnum : Enum;

        bool IsEnumDefined<TEnum>(TEnum value)
            where TEnum : Enum;

        TEnum ParseEnum<TEnum>(string value)
            where TEnum : Enum;

        bool TryParseEnum<TEnum>(string stringValue, out TEnum enumValue)
            where TEnum : struct, Enum;
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuikSharp.Tools
{
    public static class IntToStringTool
    {
        private static int _start = 0;
        private static int _end = 100;
        private static CultureInfo _cultureInfo = CultureInfo.InvariantCulture;

        private static string[] _cache = CreateCache(_start, _end, _cultureInfo);

        public static void Initialize(int start, int end, CultureInfo cultureInfo)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException($"{nameof(start)} must be less than or equal {nameof(end)}.");

            _start = start;
            _end = end;
            _cultureInfo = cultureInfo;
            _cache = CreateCache(_start, _end, _cultureInfo);
        }

        private static string[] CreateCache(int start, int end, CultureInfo cultureInfo)
        {
            var cache = new string[end - start + 1];

            for (var i = start; i <= end; i++)
            {
                cache[i - start] = i.ToString(cultureInfo);
            }

            return cache;
        }

        public static string GetString(int value)
        {
            if (_start <= value && _end >= value)
            {
                return _cache[value - _start];
            }

            return null;
        }
    }
}

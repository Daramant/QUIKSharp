using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.TypeConverters
{
    public class EnumData
    {
        public HashSet<Enum> Values { get; } = new HashSet<Enum>();

        public Dictionary<Enum, string> EnumToStringDictionary { get; } = new Dictionary<Enum, string>();

        public Dictionary<string, Enum> StringToEnumDictionary { get; } = new Dictionary<string, Enum>();
    }
}

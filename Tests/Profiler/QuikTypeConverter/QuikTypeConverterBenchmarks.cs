using BenchmarkDotNet.Attributes;
using QuikSharp.DataStructures;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.QuikTypeConverter
{
    [MemoryDiagnoser]
    public class QuikTypeConverterBenchmarks
    {
        private int _startIndex = 0;
        private int _endIndex = 100;
        private int _roundCount = 10;
        private int _enumCheckCount = 1000;

        [Benchmark(Description = "IntToString ToString(CurrentCulture)")]
        public void IntToString_ToString_CurrentCulture()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    results.Add(i.ToString());
                }
            }
        }

        [Benchmark(Description = "IntToString QuikTypeConverter.ToString()")]
        public void IntToString_QuikTypeConverter_ToString()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));
            var typeConverter = new CachingQuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    results.Add(typeConverter.ToString(i));
                }
            }
        }

        [Benchmark(Description = "IntToString QuikTypeConverter.ToStringLookup()")]
        public void IntToString_QuikTypeConverter_ToStringLookup()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));
            var typeConverter = new CachingQuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    results.Add(typeConverter.ToStringLookup(i));
                }
            }
        }

        [Benchmark(Description = "EnumToString ToString()")]
        public void EnumToString_ToString()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    results.Add(values[i].ToString());
                }
            }
        }

        [Benchmark(Description = "EnumToString QuikTypeConverter.ToString()")]
        public void EnumToString_QuikTypeConverter_ToString()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            var typeConverter = new CachingQuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    results.Add(typeConverter.ToString(values[i]));
                }
            }
        }

        [Benchmark(Description = "EnumToStringAsInt ToString()")]
        public void EnumToStringAsInt_ToString()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    results.Add(((int)values[i]).ToString());
                }
            }
        }

        [Benchmark(Description = "EnumToStringAsInt QuikTypeConverter.ToStringLookup((int)enum)")]
        public void EnumToStringAsInt_QuikTypeConverter_ToStringLookup_int_enum()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            var typeConverter = new CachingQuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    results.Add(typeConverter.ToStringLookup((int)values[i]));
                }
            }
        }

        [Benchmark(Description = "IsEnumDefined Enum.IsDefined()")]
        public void IsEnumDefined_Enum_IsDefined()
        {
            object value = TransactionAction.KILL_ALL_ORDERS;
            var results = new List<bool>(_enumCheckCount);

            for (var r = 0; r < _enumCheckCount; r++)
            {
                results.Add(Enum.IsDefined(typeof(TransactionAction), value));
            }
        }

        [Benchmark(Description = "IsEnumDefined QuikTypeConverter.IsEnumDefined")]
        public void IsEnumDefined_QuikTypeConverter_IsEnumDefined()
        {
            var value = TransactionAction.KILL_ALL_ORDERS;
            var results = new List<bool>(_enumCheckCount);

            var typeConverter = new CachingQuikTypeConverter();

            for (var r = 0; r < _enumCheckCount; r++)
            {
                results.Add(typeConverter.IsEnumDefined(value));
            }
        }
    }
}

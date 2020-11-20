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

        [Benchmark(Description = "IntToString ITypeConverter.ToString()")]
        public void IntToString_ITypeConverter_ToString()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));
            var typeConverter = new QuikSharp.TypeConverters.QuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    results.Add(typeConverter.ToString(i));
                }
            }
        }

        [Benchmark(Description = "IntToString ITypeConverter.ToStringLookup()")]
        public void IntToString_ITypeConverter_ToStringLookup()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));
            var typeConverter = new QuikSharp.TypeConverters.QuikTypeConverter();

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

        [Benchmark(Description = "EnumToString ITypeConverter.ToString()")]
        public void EnumToString_ITypeConverter_ToString()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            var typeConverter = new QuikSharp.TypeConverters.QuikTypeConverter();

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

        [Benchmark(Description = "EnumToStringAsInt ITypeConverter.ToStringLookup((int)enum)")]
        public void EnumToStringAsInt_ITypeConverter_ToStringLookup_int_enum()
        {
            var values = (TransactionAction[])Enum.GetValues(typeof(TransactionAction));
            var results = new List<string>(_roundCount * values.Length);

            var typeConverter = new QuikSharp.TypeConverters.QuikTypeConverter();

            for (var r = 0; r < _roundCount; r++)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    results.Add(typeConverter.ToStringLookup((int)values[i]));
                }
            }
        }
    }
}

using BenchmarkDotNet.Attributes;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.QuikJsonSerializer
{
    [MemoryDiagnoser]
    public class QuikJsonSerializerBenchmarks
    {
        private int _startIndex = 0;
        private int _endIndex = 100;
        private int _roundCount = 10;

        [Benchmark(Description = "IntToString ITypeConverter.ToString()")]
        public void IntToString_ITypeConverter_ToString()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));
            var jsonSerializer = new QuikSharp.Json.Serializers.QuikJsonSerializer();

            var t = new Transaction();

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    var j = jsonSerializer.Serialize(t);
                    var t2 = jsonSerializer.Deserialize<Transaction>(j);

                    results.Add(j);
                }
            }
        }
    }
}

using BenchmarkDotNet.Attributes;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Quik.Client;
using QuikSharp.Quik.Events;
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

        [Benchmark(Description = "Serialize & Deserialize QuikJsonSerializer")]
        public void QuikJsonSerializer_Serialize_Deserialize()
        {
            var results = new List<string>(_roundCount * (_endIndex - _startIndex + 1));

            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            var serializer = new QuikSharp.Serialization.Json.QuikJsonSerializer(pendingResultContainer, eventTypeProvider);

            var t = new Transaction();
            t.PRICE = 123.456m;

            for (var r = 0; r < _roundCount; r++)
            {
                for (var i = _startIndex; i <= _endIndex; i++)
                {
                    var j = serializer.Serialize(t);
                    var t2 = serializer.Deserialize<Transaction>(j);
                    
                    //if (t.PRICE != t2.PRICE)
                    //{
                    //    throw new Exception("Deserialization problem with PRICE property!");
                    //}

                    results.Add(j);
                }
            }
        }
    }
}

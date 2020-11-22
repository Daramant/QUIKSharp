using System;
using BenchmarkDotNet.Running;
using Profiler.EchoTransaction;
using Profiler.Ping;
using Profiler.QuikJsonSerializer;
using Profiler.QuikTypeConverter;

namespace Profiler
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Profiler started.");

            //PingProfiler.Ping();
            //EchoTransactionpProfiler.EchoTransaction();
            BenchmarkRunner.Run<QuikTypeConverterBenchmarks>();
            //BenchmarkRunner.Run<QuikJsonSerializerBenchmarks>();

            Console.WriteLine("Profiler finished.");
            Console.ReadKey();
        }
    }
}

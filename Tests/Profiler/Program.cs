using System;
using BenchmarkDotNet.Running;
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

            Console.WriteLine("Profiler finished.");
            Console.ReadKey();
        }
    }
}

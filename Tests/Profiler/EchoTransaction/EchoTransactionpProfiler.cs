using Profiler.Ping;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.EchoTransaction
{
    public static class EchoTransactionpProfiler
    {
        public static void EchoTransaction()
        {
            var quik = PingProfiler.CreateQuik();
            quik.Client.Start();

            const int roundCount = 10;
            var iterationCount = 10000;
            double avarage = 0d;

            var stopwatch = new Stopwatch();
            Console.WriteLine("EchoTransaction started.");

            for (int round = 0; round < roundCount; round++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                var t = new Transaction();

                var array = new Task<Transaction>[iterationCount];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = quik.Functions.Debug.EchoAsync(t);
                }
                for (int i = 0; i < array.Length; i++)
                {
                    var res = array[i].Result;
                    array[i] = null;
                }

                stopwatch.Stop();
                Console.WriteLine($"[{round}] MultiPing takes msecs: {stopwatch.ElapsedMilliseconds}; average: {(double)stopwatch.ElapsedMilliseconds * 1000 / iterationCount} microsec.");
                avarage += stopwatch.ElapsedMilliseconds;
            }

            avarage /= roundCount;
            Console.WriteLine($"[Total] MultiPing takes msecs: {avarage}; average: {(double)avarage * 1000 / iterationCount} microsec.");

            Console.WriteLine("EchoTransaction finished.");
            Console.WriteLine();
        }
    }
}

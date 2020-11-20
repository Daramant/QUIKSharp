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

            var stopwatch = new Stopwatch();
            Console.WriteLine("EchoTransaction started.");

            for (int round = 0; round < 10; round++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                var count = 10000;
                var t = new Transaction();

                var array = new Task<Transaction>[count];
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
                Console.WriteLine("MultiPing takes msecs: " + stopwatch.ElapsedMilliseconds);
            }

            Console.WriteLine("EchoTransaction finished.");
            Console.WriteLine();
        }
    }
}

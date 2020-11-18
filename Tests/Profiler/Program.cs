using System;
using System.Diagnostics;
using System.Threading.Tasks;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Quik;
using QuikSharp.QuikClient;

namespace Profiler
{
    public class Program
    {
        private static IQuik CreateQuik()
        {
            var quikFactory = new QuikFactory();

            var options = QuikClientOptions.GetDefault();
            return quikFactory.Create(options);
        }

        public static void Ping()
        {
            var quik = CreateQuik();
            quik.Client.Start();

            const int roundCount = 10;
            var iterationCount = 10000;
            double avarage = 0d;
            var stopwatch = new Stopwatch();
            Console.WriteLine("Started");
            for (int round = 0; round < roundCount; round++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                var array = new Task<string>[iterationCount];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = quik.Functions.Debug.PingAsync();
                }
                for (int i = 0; i < array.Length; i++)
                {
                    var pong = array[i].Result;
                    array[i] = null;
                    Trace.Assert(pong == "Pong");
                }

                //for (var i = 0; i < count; i++) {
                //    var pong = qc.Ping().Result;
                //    Trace.Assert(pong == "Pong");
                //}
                stopwatch.Stop();
                Console.WriteLine($"[{round}] MultiPing takes msecs: {stopwatch.ElapsedMilliseconds}; average: {(double)stopwatch.ElapsedMilliseconds * 1000 / iterationCount} microsec.");
                avarage += stopwatch.ElapsedMilliseconds;
            }

            avarage /= roundCount;
            Console.WriteLine($"[Total] MultiPing takes msecs: {avarage}; average: {(double)avarage * 1000 / iterationCount} microsec.");

            Console.WriteLine("Finished");
            Console.ReadKey();
        }


        public static void EchoTransaction()
        {
            var quik = CreateQuik();
            quik.Client.Start();

            var stopwatch = new Stopwatch();
            Console.WriteLine("Started");
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
            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        public static void Main()
        {
            Ping();
            //EchoTransaction();
        }
    }
}

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Quik;
using QuikSharp.QuikService;

namespace Profiler
{
    public class Program
    {
        private static IQuik CreateQuik()
        {
            var quikFactory = new QuikFactory();

            var options = QuikServiceOptions.GetDefault();
            return quikFactory.Create(options);
        }

        public static void Ping()
        {
            var quik = CreateQuik();
            quik.Service.Start();

            var stopwatch = new Stopwatch();
            Console.WriteLine("Started");
            for (int round = 0; round < 10; round++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                var count = 10000;
                var array = new Task<string>[count];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = quik.Functions.Debug.Ping();
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
                Console.WriteLine("MultiPing takes msecs: " + stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine("Finished");
            Console.ReadKey();
        }


        public static void EchoTransaction()
        {
            var quik = CreateQuik();
            quik.Service.Start();

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
                    array[i] = quik.Functions.Debug.Echo(t);
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

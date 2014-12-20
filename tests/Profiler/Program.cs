﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuikSharp.Quik;

namespace Profiler {
    public class Program {
        public static void Main() {
            var qc = new DebugFunctions();
            var sw = new Stopwatch();
            Console.WriteLine("Started");
            for (int round = 0; round < 10; round++) {
                sw.Reset();
                sw.Start();
                
                var count = 10000;
                var array = new Task<string>[count];
                for (int i = 0; i < array.Length; i++) {
                    array[i] = qc.Ping();
                }
                for (int i = 0; i < array.Length; i++) {
                    var pong = array[i].Result;
                    array[i] = null;
                    Trace.Assert(pong == "Pong");
                }

                //for (var i = 0; i < count; i++) {
                //    var pong = qc.Ping().Result;
                //    Trace.Assert(pong == "Pong");
                //}
                sw.Stop();
                Console.WriteLine("MultiPing takes msecs: " + sw.ElapsedMilliseconds);
            }
            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
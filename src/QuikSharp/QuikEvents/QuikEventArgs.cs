using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikEvents
{
    public class InitEventArgs : EventArgs
    {
        public int Port { get; set; }

        public InitEventArgs()
        { }

        public InitEventArgs(int port)
        {
            Port = port;
        }
    }

    public class StopEventArgs : EventArgs
    {
        public int Signal { get; set; }

        public StopEventArgs()
        { }

        public StopEventArgs(int signal)
        {
            Signal = signal;
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string Error { get; set; }

        public ErrorEventArgs()
        { }

        public ErrorEventArgs(string error)
        {
            Error = error;
        }
    }
}

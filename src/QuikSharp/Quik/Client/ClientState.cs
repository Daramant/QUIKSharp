using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Client
{
    public enum ClientState
    {
        Undefined = 0,

        Starting, 

        Started,

        Stopping, 

        Stopped,
    }
}

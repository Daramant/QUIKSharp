using QuikSharp.Quik.Client;
using QuikSharp.Quik.Events;
using QuikSharp.Quik.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuik
    {
        IQuikClient Client { get; }

        IQuikFunctions Functions { get; }

        IQuikEvents Events { get; }
    }
}

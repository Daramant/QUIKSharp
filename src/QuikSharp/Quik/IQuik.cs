using QuikSharp.QuikClient;
using QuikSharp.QuikEvents;
using QuikSharp.QuikFunctions;
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

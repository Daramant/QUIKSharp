using Microsoft.Extensions.Logging;
using QuikSharp.Quik.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuikFactory
    {
        void ConfigureLogging(Action<ILoggingBuilder> configure);

        IQuik Create(QuikClientOptions options);
    }
}

using Microsoft.Extensions.Logging;
using QuikSharp.Quik;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Transactions
{
    public interface ITransactionManagerFactory
    {
        void ConfigureLogging(Action<ILoggingBuilder> configure);

        ITransactionManager Create(IQuik quik, TransactionManagerOptions options);
    }
}

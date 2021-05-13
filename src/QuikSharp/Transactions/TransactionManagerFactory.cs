using Microsoft.Extensions.Logging;
using QuikSharp.PersistentStorages;
using QuikSharp.Quik;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Transactions
{
    public class TransactionManagerFactory : ITransactionManagerFactory
    {
        private ILoggerFactory _loggerFactory = LoggerFactory.Create(b => { });

        /// <inheritdoc/>
        public void ConfigureLogging(Action<ILoggingBuilder> configure)
        {
            _loggerFactory = LoggerFactory.Create(configure);
        }

        /// <inheritdoc/>
        public ITransactionManager Create(IQuik quik, TransactionManagerOptions options)
        {
            var idProvider = new TransactionIdProvider();
            var persistentStorage = new InMemoryPersistantStorage();

            return new TransactionManager(quik, idProvider, persistentStorage, _loggerFactory.CreateLogger<TransactionManager>());
        }
    }
}

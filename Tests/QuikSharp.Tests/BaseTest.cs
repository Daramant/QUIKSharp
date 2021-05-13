using QuikSharp.Serialization.Json.Converters;
using QuikSharp.Quik;
using QuikSharp.Quik.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuikSharp.Serialization;
using QuikSharp.Serialization.Json;
using QuikSharp.Quik.Events;
using QuikSharp.Transactions;

namespace QuikSharp.Tests
{
    public abstract class BaseTest
    {
        protected IQuik Quik { get; private set; }

        protected ITransactionManager TransactionManager { get; private set; }

        protected IQuikFactory QuikFactory { get; private set; }

        protected ISerializer Serializer { get; private set; }

        protected void SetUpQuik()
        {
            QuikFactory = new QuikFactory();

            var quikClientOptions = QuikClientOptions.GetDefault();
            Quik = QuikFactory.Create(quikClientOptions);

            Quik.Client.Start();

            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            Serializer = new QuikJsonSerializer(pendingResultContainer, eventTypeProvider);

            var transactionManagerFactory = new TransactionManagerFactory();
            TransactionManager = transactionManagerFactory.Create(Quik, TransactionManagerOptions.GetDefault());
        }
    }
}

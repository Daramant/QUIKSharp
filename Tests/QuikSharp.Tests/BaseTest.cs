using QuikSharp.Serialization.Json.Converters;
using QuikSharp.Quik;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuikSharp.Serialization;
using QuikSharp.Serialization.Json;
using QuikSharp.QuikEvents;

namespace QuikSharp.Tests
{
    public abstract class BaseTest
    {
        protected IQuik Quik { get; private set; }

        protected IQuikFactory QuikFactory { get; private set; }

        protected ISerializer Serializer { get; private set; }

        protected void SetUpQuik()
        {
            QuikFactory = new QuikFactory();

            var options = QuikClientOptions.GetDefault();
            Quik = QuikFactory.Create(options);

            Quik.Client.Start();

            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            Serializer = new QuikJsonSerializer(pendingResultContainer, eventTypeProvider);
        }
    }
}

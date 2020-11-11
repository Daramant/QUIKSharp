using QuikSharp.Json.Converters;
using QuikSharp.Json.Serializers;
using QuikSharp.Quik;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Tests
{
    public abstract class BaseTest
    {
        protected IQuik Quik { get; private set; }

        protected IQuikFactory QuikFactory { get; private set; }

        protected IQuikJsonSerializer JsonSerializer { get; private set; }

        protected void SetUpQuik()
        {
            QuikFactory = new QuikFactory();

            var options = QuikClientOptions.GetDefault();
            Quik = QuikFactory.Create(options);

            Quik.Client.Start();

            JsonSerializer = new QuikJsonSerializer();

            JsonSerializer.AddConverter(new MessageConverter((QuikClient.QuikClient)Quik.Client));
        }
    }
}

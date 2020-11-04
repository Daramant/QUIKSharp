using QuikSharp.Json.Converters;
using QuikSharp.Json.Serializers;
using QuikSharp.Quik;
using QuikSharp.QuikService;
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

        protected IJsonSerializer JsonSerializer { get; private set; }

        protected void SetUpQuik()
        {
            QuikFactory = new QuikFactory();

            var options = QuikServiceOptions.GetDefault();
            Quik = QuikFactory.Create(options);

            Quik.Service.Start();

            JsonSerializer = new JsonSerializer();

            JsonSerializer.AddConverter(new MessageConverter((QuikService.QuikService)Quik.Service));
        }
    }
}

using QuikSharp.Json.Converters;
using QuikSharp.Json.Serializers;
using QuikSharp.PersistentStorages;
using QuikSharp.QuikFunctions.Candles;
using QuikSharp.QuikFunctions.Classes;
using QuikSharp.QuikFunctions.Debug;
using QuikSharp.QuikFunctions.OrderBooks;
using QuikSharp.QuikFunctions.Orders;
using QuikSharp.QuikFunctions.Services;
using QuikSharp.QuikFunctions.StopOrders;
using QuikSharp.QuikFunctions.Trading;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Text;
using QuikSharp.QuikFunctions.Labels;
using QuikSharp.TypeConverters;

namespace QuikSharp.Quik
{
    public class QuikFactory : IQuikFactory
    {
        public IQuik Create(QuikClientOptions options)
        {
            var jsonSerializer = new QuikJsonSerializer();
            var persistentStorage = new InMemoryPersistantStorage();
            var typeConverter = new QuikTypeConverter();

            var quikEvents = new QuikEvents.QuikEvents(persistentStorage, typeConverter);
            var quikEventHandler = new QuikEventHandler(quikEvents);
            var quikClient = new QuikClient.QuikClient(quikEventHandler, jsonSerializer, options);
            var tradingFunctions = new TradingFunctions(quikClient, persistentStorage, typeConverter);

            jsonSerializer.AddConverter(new MessageConverter(quikClient));

            return new Quik(
                quikClient,
                new QuikFunctions.QuikFunctions(
                    new DebugFunctions(quikClient),
                    new ServiceFunctions(quikClient, typeConverter),
                    new ClassFunctions(quikClient),
                    new OrderFunctions(quikClient, tradingFunctions, typeConverter),
                    new OrderBookFunctions(quikClient),
                    new StopOrderFunctions(quikClient, tradingFunctions, typeConverter),
                    tradingFunctions,
                    new CandleFunctions(quikClient, typeConverter),
                    new LabelFunctions(quikClient, typeConverter)),
                new QuikEvents.QuikEvents(persistentStorage, typeConverter));
        }
    }
}

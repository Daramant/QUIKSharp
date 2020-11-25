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
using QuikSharp.IdProviders;

namespace QuikSharp.Quik
{
    public class QuikFactory : IQuikFactory
    {
        public IQuik Create(QuikClientOptions options)
        {
            var jsonSerializer = new QuikJsonSerializer();
            var persistentStorage = new InMemoryPersistantStorage();
            var typeConverter = new CachingQuikTypeConverter();
            var idProvider = new IdProvider();

            var quikEvents = new QuikEvents.QuikEvents();
            var quikEventHandler = new EventInvoker(typeConverter, quikEvents);
            var quikClient = new QuikClient.QuikClient(quikEventHandler, jsonSerializer, idProvider, options);
            var tradingFunctions = new TradingFunctions(quikClient, persistentStorage, typeConverter, idProvider);

            jsonSerializer.AddConverter(new MessageConverter(quikClient));

            var quik = new Quik(
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
                quikEvents);

            quikClient.SetEventSender(quik);
            quikEvents.SetEventSender(quik);

            return quik;
        }
    }
}

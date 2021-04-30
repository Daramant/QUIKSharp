using QuikSharp.PersistentStorages;
using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.Classes;
using QuikSharp.Quik.Functions.Debug;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.Orders;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.Quik.Functions.StopOrders;
using QuikSharp.Quik.Functions.Trading;
using QuikSharp.Quik.Client;
using System;
using System.Collections.Generic;
using System.Text;
using QuikSharp.Quik.Functions.Labels;
using QuikSharp.TypeConverters;
using QuikSharp.Providers;
using QuikSharp.Serialization.Json;
using QuikSharp.Quik.Events;

namespace QuikSharp.Quik
{
    public class QuikFactory : IQuikFactory
    {
        public IQuik Create(QuikClientOptions options)
        {
            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            var serializer = new QuikJsonSerializer(pendingResultContainer, eventTypeProvider);
            var persistentStorage = new InMemoryPersistantStorage();
            var typeConverter = new CachingQuikTypeConverter();
            var idProvider = new IdProvider();
            var dateTimeProvider = new CurrentDateTimeProvider();

            var quikEvents = new QuikEvents();
            var eventHandler = new EventInvoker(quikEvents);
            var quikClient = new QuikClient(eventHandler, serializer, idProvider, dateTimeProvider, pendingResultContainer, options);
            var tradingFunctions = new TradingFunctions(quikClient, persistentStorage, typeConverter, idProvider);

            var quik = new Quik(
                quikClient,
                new Functions.QuikFunctions(
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

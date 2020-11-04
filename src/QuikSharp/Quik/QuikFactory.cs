using QuikSharp.Json.Serializers;
using QuikSharp.PersistentStorages;
using QuikSharp.QuikFunctions.Candles;
using QuikSharp.QuikFunctions.Classes;
using QuikSharp.QuikFunctions.Debugs;
using QuikSharp.QuikFunctions.OrderBooks;
using QuikSharp.QuikFunctions.Orders;
using QuikSharp.QuikFunctions.Services;
using QuikSharp.QuikFunctions.StopOrders;
using QuikSharp.QuikFunctions.Tradings;
using QuikSharp.QuikService;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public class QuikFactory : IQuikFactory
    {
        public IQuik Create(QuikServiceOptions options)
        {
            var jsonSerializer = new JsonSerializer();
            var persistentStorage = new InMemoryPersistantStorage();

            var quikEvents = new QuikEvents.QuikEvents(persistentStorage);
            var quikService = new QuikService.QuikService(quikEvents, jsonSerializer, options);
            var tradingFunctions = new TradingFunctions(quikService, persistentStorage);

            return new Quik(
                quikService,
                new QuikFunctions.QuikFunctions(
                    new DebugFunctions(quikService),
                    new ServiceFunctions(quikService),
                    new ClassFunctions(quikService),
                    new OrderFunctions(quikService, tradingFunctions),
                    new OrderBookFunctions(quikService),
                    new StopOrderFunctions(quikService, tradingFunctions),
                    tradingFunctions,
                    new CandleFunctions(quikService)),
                new QuikEvents.QuikEvents(persistentStorage));
        }
    }
}

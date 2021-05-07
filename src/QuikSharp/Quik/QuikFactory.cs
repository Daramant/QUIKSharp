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
using Microsoft.Extensions.Logging;
using QuikSharp.Messages;

namespace QuikSharp.Quik
{
    public class QuikFactory : IQuikFactory
    {
        private ILoggerFactory _loggerFactory = LoggerFactory.Create(b => { });

        /// <inheritdoc/>
        public void ConfigureLogging(Action<ILoggingBuilder> configure)
        {
            _loggerFactory = LoggerFactory.Create(configure);
        }

        /// <inheritdoc/>
        public IQuik Create(QuikClientOptions options)
        {
            var pendingResultContainer = new PendingResultContainer();
            var eventTypeProvider = new EventTypeProvider();
            var serializer = new QuikJsonSerializer(pendingResultContainer, eventTypeProvider);
            var persistentStorage = new InMemoryPersistantStorage();
            var typeConverter = new CachingQuikTypeConverter();
            var idProvider = new IdProvider();
            var dateTimeProvider = new CurrentDateTimeProvider();
            var messageFactory = new MessageFactory(idProvider, dateTimeProvider);

            var quikEvents = new QuikEvents();
            var eventHandler = new EventInvoker(quikEvents);
            var quikClient = new QuikClient(eventHandler, serializer, dateTimeProvider, 
                pendingResultContainer, options, _loggerFactory.CreateLogger<QuikClient>());

            var tradingFunctions = new TradingFunctions(messageFactory, quikClient, typeConverter, persistentStorage, idProvider);

            var quik = new Quik(
                quikClient,
                new Functions.QuikFunctions(
                    new DebugFunctions(messageFactory, quikClient, typeConverter),
                    new ServiceFunctions(messageFactory, quikClient, typeConverter),
                    new ClassFunctions(messageFactory, quikClient, typeConverter),
                    new OrderFunctions(messageFactory, quikClient, typeConverter, tradingFunctions),
                    new OrderBookFunctions(messageFactory, quikClient, typeConverter),
                    new StopOrderFunctions(messageFactory, quikClient, typeConverter, tradingFunctions),
                    tradingFunctions,
                    new CandleFunctions(messageFactory, quikClient, typeConverter),
                    new LabelFunctions(messageFactory, quikClient, typeConverter)),
                quikEvents);

            quikClient.SetEventSender(quik);
            quikEvents.SetEventSender(quik);

            return quik;
        }
    }
}

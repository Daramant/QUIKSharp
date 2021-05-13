using QuikSharp.PersistentStorages;
using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.Classes;
using QuikSharp.Quik.Functions.Debug;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.Services;
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
using QuikSharp.Quik.Functions.Custom;
using QuikSharp.Quik.Functions.TableRows;
using QuikSharp.Quik.Functions.Workplace;
using QuikSharp.Quik.Functions.QuotesTableParameters;

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
            var typeConverter = new CachingQuikTypeConverter();
            var idProvider = new UniqueIdProvider();
            var dateTimeProvider = new CurrentDateTimeProvider();
            var messageFactory = new MessageFactory(idProvider, dateTimeProvider);

            var quikEvents = new QuikEvents();
            var eventHandler = new EventInvoker(quikEvents);
            var quikClient = new QuikClient(eventHandler, serializer, pendingResultContainer, options, 
                _loggerFactory.CreateLogger<QuikClient>());

            var quik = new Quik(
                quikClient,
                new Functions.QuikFunctions(
                    new ServiceFunctions(messageFactory, quikClient, typeConverter),
                    new TableRowFunctions(messageFactory, quikClient, typeConverter),
                    new ClassFunctions(messageFactory, quikClient, typeConverter),
                    new WorkstationFunctions(messageFactory, quikClient, typeConverter),
                    new CandleFunctions(messageFactory, quikClient, typeConverter),
                    new LabelFunctions(messageFactory, quikClient, typeConverter),
                    new OrderBookFunctions(messageFactory, quikClient, typeConverter),
                    new QuotesTableParametersFunctions(messageFactory, quikClient, typeConverter),
                    new DebugFunctions(messageFactory, quikClient, typeConverter),
                    new CustomFunctions(messageFactory, quikClient, typeConverter)),
                quikEvents);

            quikClient.SetEventSender(quik);
            quikEvents.SetEventSender(quik);

            return quik;
        }
    }
}

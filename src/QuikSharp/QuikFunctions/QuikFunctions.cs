using QuikSharp.QuickFunctions.Candles;
using QuikSharp.QuickFunctions.Classes;
using QuikSharp.QuickFunctions.Debugs;
using QuikSharp.QuickFunctions.OrderBooks;
using QuikSharp.QuickFunctions.Orders;
using QuikSharp.QuickFunctions.Services;
using QuikSharp.QuickFunctions.StopOrders;
using QuikSharp.QuickFunctions.Tradings;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikFunctions
{
    public class QuikFunctions : IQuikFunctions
    {
        /// <summary>
        /// Debug functions
        /// </summary>
        public IDebugFunctions Debug { get; }

        /// <summary>
        /// Сервисные функции
        /// </summary>
        public IServiceFunctions Service { get; }

        /// <summary>
        /// Функции для обращения к спискам доступных параметров
        /// </summary>
        public IClassFunctions Class { get; }

        /// <summary>
        /// Функции для работы со стаканом котировок
        /// </summary>
        public IOrderBookFunctions OrderBook { get; }

        /// <summary>
        /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
        /// </summary>
        public ITradingFunctions Trading { get; }

        /// <summary>
        /// Функции для работы со стоп-заявками
        /// </summary>
        public IStopOrderFunctions StopOrders { get; }

        /// <summary>
        /// Функции для работы с заявками
        /// </summary>
        public IOrderFunctions Orders { get; }

        /// <summary>
        /// Функции для работы со свечами
        /// </summary>
        public ICandleFunctions Candles { get; }

        public QuikFunctions(
            IDebugFunctions debugFunctions,
            IServiceFunctions serviceFunctions,
            IClassFunctions classFunctions,
            IOrderFunctions orderFunctions,
            IOrderBookFunctions orderBookFunctions,
            IStopOrderFunctions stopOrderFunctions,
            ITradingFunctions tradingFunctions,
            ICandleFunctions candleFunctions
            )
        {
            Debug = debugFunctions;
            Service = serviceFunctions;
            Class = classFunctions;
            OrderBook = orderBookFunctions;
            Trading = tradingFunctions;
            StopOrders = stopOrderFunctions;
            Orders = orderFunctions;
            Candles = candleFunctions;
        }
    }
}

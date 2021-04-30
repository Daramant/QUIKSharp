using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.Classes;
using QuikSharp.Quik.Functions.Debug;
using QuikSharp.Quik.Functions.Labels;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.Orders;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.Quik.Functions.StopOrders;
using QuikSharp.Quik.Functions.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Functions
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

        /// <summary>
        /// Функции для работы с метками
        /// </summary>
        public ILabelFunctions Labels { get; }

        public QuikFunctions(
            IDebugFunctions debugFunctions,
            IServiceFunctions serviceFunctions,
            IClassFunctions classFunctions,
            IOrderFunctions orderFunctions,
            IOrderBookFunctions orderBookFunctions,
            IStopOrderFunctions stopOrderFunctions,
            ITradingFunctions tradingFunctions,
            ICandleFunctions candleFunctions,
            ILabelFunctions labelFunctions
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
            Labels = labelFunctions;
        }
    }
}

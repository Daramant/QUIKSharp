using QuikSharp.CandleFunctions;
using QuikSharp.ClassFunctions;
using QuikSharp.DebugFunctions;
using QuikSharp.OrderBookFunctions;
using QuikSharp.OrderFunctions;
using QuikSharp.QuickService;
using QuikSharp.QuikEvents;
using QuikSharp.ServiceFunctions;
using QuikSharp.StopOrderFunctions;
using QuikSharp.TradingFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp
{
    public interface IQuick
    {
        IQuikService QuikService { get; }

        /// <summary>
        /// Persistent transaction storage
        /// </summary>
        IPersistentStorage Storage { get; }

        /// <summary>
        /// Debug functions
        /// </summary>
        IDebugFunctions Debug { get; }

        /// <summary>
        /// Функции обратного вызова
        /// </summary>
        IQuikEvents Events { get; }

        /// <summary>
        /// Сервисные функции
        /// </summary>
        IServiceFunctions Service { get; }

        /// <summary>
        /// Функции для обращения к спискам доступных параметров
        /// </summary>
        IClassFunctions Class { get; }

        /// <summary>
        /// Функции для работы со стаканом котировок
        /// </summary>
        IOrderBookFunctions OrderBook { get; }

        /// <summary>
        /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
        /// </summary>
        ITradingFunctions Trading { get; }

        /// <summary>
        /// Функции для работы со стоп-заявками
        /// </summary>
        IStopOrderFunctions StopOrders { get; }

        /// <summary>
        /// Функции для работы с заявками
        /// </summary>
        IOrderFunctions Orders { get; }

        /// <summary>
        /// Функции для работы со свечами
        /// </summary>
        ICandleFunctions Candles { get; }
    }
}

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
    public interface IQuikFunctions
    {
        /// <summary>
        /// Debug functions
        /// </summary>
        IDebugFunctions Debug { get; }

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

        /// <summary>
        /// Функции для работы с метками
        /// </summary>
        ILabelFunctions Labels { get; }
    }
}

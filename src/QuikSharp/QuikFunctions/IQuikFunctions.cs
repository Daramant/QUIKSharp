﻿using QuikSharp.QuickFunctions.Candles;
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
    }
}
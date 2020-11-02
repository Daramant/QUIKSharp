// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.CandleFunctions;
using QuikSharp.ClassFunctions;
using QuikSharp.DebugFunctions;
using QuikSharp.OrderBookFunctions;
using QuikSharp.OrderFunctions;
using QuikSharp.QuickService;
using QuikSharp.ServiceFunctions;
using QuikSharp.StopOrderFunctions;
using QuikSharp.TradingFunctions;
using System;

namespace QuikSharp
{
    /// <summary>
    /// Quik interface in .NET
    /// </summary>
    public sealed class Quik : IQuick
    {
        /// <summary>
        /// 34130
        /// </summary>
        public const int DefaultPort = 34130;

        /// <summary>
        /// localhost
        /// </summary>
        public const string DefaultHost = "127.0.0.1";

        public IQuikService QuikService { get; }

        /// <summary>
        /// Persistent transaction storage
        /// </summary>
        public IPersistentStorage Storage { get; }

        /// <summary>
        /// Debug functions
        /// </summary>
        public IDebugFunctions Debug { get; }

        /// <summary>
        /// Функции обратного вызова
        /// </summary>
        public IQuikEvents Events { get; }

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
        /// Quik interface in .NET constructor
        /// </summary>
        public Quik(
            IQuikService quickService, 
            IPersistentStorage storage,
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
            
            QuikService = quickService;
            Storage = storage;

            Events = QuikService.Events;
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
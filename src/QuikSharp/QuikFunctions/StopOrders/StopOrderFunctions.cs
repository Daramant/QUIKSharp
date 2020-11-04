﻿// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using QuikSharp.QuickFunctions.Tradings;
using QuikSharp.QuickService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.QuickFunctions.StopOrders
{
    /// <summary>
    /// Функции для работы со стоп-заявками
    /// </summary>
    public class StopOrderFunctions : IStopOrderFunctions
    {
        private readonly IQuikService _quikService;
        private readonly ITradingFunctions _tradingFunctions;

        public StopOrderFunctions(
            IQuikService quikService,
            ITradingFunctions tradingFunctions)
        {
            _quikService = quikService;
            _tradingFunctions = tradingFunctions;
        }

        /// <summary>
        /// Возвращает список всех стоп-заявок.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StopOrder>> GetStopOrders()
        {
            var message = new Request<string>("", "get_stop_orders");
            Message<List<StopOrder>> response = await _quikService.Send<Response<List<StopOrder>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список стоп-заявок для заданного инструмента.
        /// </summary>
        public async Task<List<StopOrder>> GetStopOrders(string classCode, string securityCode)
        {
            var message = new Request<string>(classCode + "|" + securityCode, "get_stop_orders");
            Message<List<StopOrder>> response = await _quikService.Send<Response<List<StopOrder>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<long> CreateStopOrder(StopOrder stopOrder)
        {
            Transaction newStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_STOP_ORDER,
                ACCOUNT = stopOrder.Account,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                EXPIRY_DATE = "GTC", //до отмены
                STOPPRICE = stopOrder.ConditionPrice,
                PRICE = stopOrder.Price,
                QUANTITY = stopOrder.Quantity,
                STOP_ORDER_KIND = ConvertStopOrderType(stopOrder.StopOrderType),
                OPERATION = stopOrder.Operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S
            };
            if (stopOrder.StopOrderType == StopOrderType.TakeProfit || stopOrder.StopOrderType == StopOrderType.TakeProfitStopLimit)
            {
                newStopOrderTransaction.OFFSET = stopOrder.Offset;
                newStopOrderTransaction.SPREAD = stopOrder.Spread;
                newStopOrderTransaction.OFFSET_UNITS = stopOrder.OffsetUnit;
                newStopOrderTransaction.SPREAD_UNITS = stopOrder.SpreadUnit;
            }

            if (stopOrder.StopOrderType == StopOrderType.TakeProfitStopLimit)
            {
                newStopOrderTransaction.STOPPRICE2 = stopOrder.ConditionPrice2;
            }

            //todo: Not implemented
            //["OFFSET"]=tostring(SysFunc.toPrice(SecCode,MaxOffset)),
            //["OFFSET_UNITS"]="PRICE_UNITS",
            //["SPREAD"]=tostring(SysFunc.toPrice(SecCode,DefSpread)),
            //["SPREAD_UNITS"]="PRICE_UNITS",
            //["MARKET_STOP_LIMIT"]="YES",
            //["MARKET_TAKE_PROFIT"]="YES",
            //["STOPPRICE2"]=tostring(SysFunc.toPrice(SecCode,StopLoss)),
            //["EXECUTION_CONDITION"] = "FILL_OR_KILL",

            return await _tradingFunctions.SendTransaction(newStopOrderTransaction).ConfigureAwait(false);
        }

        private StopOrderKind ConvertStopOrderType(StopOrderType stopOrderType)
        {
            switch (stopOrderType)
            {
                case StopOrderType.StopLimit:
                    return StopOrderKind.SIMPLE_STOP_ORDER;

                case StopOrderType.TakeProfit:
                    return StopOrderKind.TAKE_PROFIT_STOP_ORDER;

                case StopOrderType.TakeProfitStopLimit:
                    return StopOrderKind.TAKE_PROFIT_AND_STOP_LIMIT_ORDER;

                default:
                    throw new Exception("Not implemented stop order type: " + stopOrderType);
            }
        }

        public async Task<long> KillStopOrder(StopOrder stopOrder)
        {
            Transaction killStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_STOP_ORDER,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                STOP_ORDER_KEY = stopOrder.OrderNum.ToString()
            };
            return await _tradingFunctions.SendTransaction(killStopOrderTransaction).ConfigureAwait(false);
        }
    }
}
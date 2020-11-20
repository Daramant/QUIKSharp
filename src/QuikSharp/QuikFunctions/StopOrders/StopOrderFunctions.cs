// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using QuikSharp.QuikFunctions.Trading;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuikSharp.Extensions;
using QuikSharp.TypeConverters;

namespace QuikSharp.QuikFunctions.StopOrders
{
    /// <summary>
    /// Функции для работы со стоп-заявками
    /// </summary>
    public class StopOrderFunctions : IStopOrderFunctions
    {
        private readonly IQuikClient _quikClient;
        private readonly ITradingFunctions _trading;
        private readonly ITypeConverter _typeConverter;

        public StopOrderFunctions(
            IQuikClient quikClient,
            ITradingFunctions trading,
            ITypeConverter typeConverter)
        {
            _quikClient = quikClient;
            _trading = trading;
            _typeConverter = typeConverter;
        }

        /// <summary>
        /// Возвращает список всех стоп-заявок.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StopOrder>> GetStopOrdersAsync()
        {
            var message = new Command<string>(string.Empty, "get_stop_orders");
            var response = await _quikClient.SendAsync<Result<List<StopOrder>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список стоп-заявок для заданного инструмента.
        /// </summary>
        public async Task<List<StopOrder>> GetStopOrdersAsync(string classCode, string securityCode)
        {
            var message = new Command<string[]>(new[] { classCode, securityCode }, "get_stop_orders");
            var response = await _quikClient.SendAsync<Result<List<StopOrder>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        public Task<long> CreateStopOrderAsync(StopOrder stopOrder)
        {
            var newStopOrderTransaction = new Transaction
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

            return _trading.SendTransactionAsync(newStopOrderTransaction);
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

        public Task<long> KillStopOrderAsync(StopOrder stopOrder)
        {
            var killStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_STOP_ORDER,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                STOP_ORDER_KEY = _typeConverter.ToString(stopOrder.OrderNum)
            };

            return _trading.SendTransactionAsync(killStopOrderTransaction);
        }
    }
}
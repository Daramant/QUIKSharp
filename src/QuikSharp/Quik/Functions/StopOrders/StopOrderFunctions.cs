// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using QuikSharp.Quik.Functions.Trading;
using QuikSharp.Quik.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuikSharp.Extensions;
using QuikSharp.TypeConverters;

namespace QuikSharp.Quik.Functions.StopOrders
{
    /// <summary>
    /// Функции для работы со стоп-заявками
    /// </summary>
    public class StopOrderFunctions : FunctionsBase, IStopOrderFunctions
    {
        private readonly ITradingFunctions _tradingFunctions;

        public StopOrderFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter,
            ITradingFunctions tradingFunctions)
            : base(messageFactory, quikClient, typeConverter)
        {
            _tradingFunctions = tradingFunctions;
        }

        

        /// <inheritdoc/>
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

            return _tradingFunctions.SendTransactionAsync(newStopOrderTransaction);
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

        /// <inheritdoc/>
        public Task<long> KillStopOrderAsync(StopOrder stopOrder)
        {
            var killStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_STOP_ORDER,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                STOP_ORDER_KEY = TypeConverter.ToString(stopOrder.OrderNum)
            };

            return _tradingFunctions.SendTransactionAsync(killStopOrderTransaction);
        }
    }
}
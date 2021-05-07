// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using QuikSharp.Quik.Client;
using QuikSharp.Messages;
using QuikSharp.Quik.Functions.Trading;
using QuikSharp.TypeConverters;

namespace QuikSharp.Quik.Functions.Orders
{
    /// <summary>
    /// Класс, содержащий методы работы с заявками.
    /// </summary>
    public class OrderFunctions : FunctionsBase, IOrderFunctions
    {
        private readonly ITradingFunctions _tradingFunctions;

        public OrderFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter,
            ITradingFunctions tradingFunctions)
            : base (messageFactory, quikClient, typeConverter)
        {
            _tradingFunctions = tradingFunctions;
        }

        /// <inheritdoc/>
        public Task<long> CreateOrderAsync(Order order)
        {
            Transaction newOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_ORDER,
                ACCOUNT = order.Account,
                CLASSCODE = order.ClassCode,
                SECCODE = order.SecCode,
                QUANTITY = order.Quantity,
                OPERATION = order.Operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S,
                PRICE = order.Price,
                CLIENT_CODE = order.ClientCode
            };
            return _tradingFunctions.SendTransactionAsync(newOrderTransaction);
        }

        /// <inheritdoc/>
        public Task<Order> SendLimitOrderAsync(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty)
        {
            return SendOrder(classCode, securityCode, accountID, operation, price, qty, TransactionType.L);
        }

        /// <inheritdoc/>
        public Task<Order> SendMarketOrderAsync(string classCode, string securityCode, string accountID, Operation operation, int qty)
        {
            return SendOrder(classCode, securityCode, accountID, operation, 0, qty, TransactionType.M);
        }

        /// <inheritdoc/>
        private async Task<Order> SendOrder(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty, TransactionType orderType)
        {
            long res = 0;
            bool set = false;
            Order order_result = new Order();
            Transaction newOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_ORDER,
                ACCOUNT = accountID,
                CLASSCODE = classCode,
                SECCODE = securityCode,
                QUANTITY = qty,
                OPERATION = operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S,
                PRICE = price,
                TYPE = orderType
            };
            try
            {
                res = await _tradingFunctions.SendTransactionAsync(newOrderTransaction).ConfigureAwait(false);
                Thread.Sleep(500);
                Console.WriteLine("res: " + res);
            }
            catch
            {
                //ignore
            }

            while (!set)
            {
                if (res > 0)
                {
                    try
                    {
                        order_result = await GetOrderByTransactionIdAsync(classCode, securityCode, res).ConfigureAwait(false);
                    }
                    catch
                    {
                        order_result = new Order {RejectReason = "Неудачная попытка получения заявки по ID-транзакции №" + res};
                    }
                }
                else
                {
                    if (order_result != null) order_result.RejectReason = newOrderTransaction.ErrorMessage;
                    else order_result = new Order {RejectReason = newOrderTransaction.ErrorMessage};
                }

                if (order_result != null && (order_result.RejectReason != "" || order_result.OrderNum > 0)) set = true;
            }

            return order_result;
        }

        /// <inheritdoc/>
        public Task<long> KillOrderAsync(Order order)
        {
            var killOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_ORDER,
                CLASSCODE = order.ClassCode,
                SECCODE = order.SecCode,
                ORDER_KEY = TypeConverter.ToString(order.OrderNum)
            };
            return _tradingFunctions.SendTransactionAsync(killOrderTransaction);
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderAsync(string classCode, long orderId)
        {
            return ExecuteCommandAsync<Order>("get_order_by_number", new[] { classCode, TypeConverter.ToString(orderId) });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Order>> GetOrdersAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<Order>>("get_orders", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Order>> GetOrdersAsync(string classCode, string securityCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Order>>("get_orders", new[] { classCode, securityCode });
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderByTransactionIdAsync(string classCode, string securityCode, long trans_id)
        {
            return ExecuteCommandAsync<Order>("getOrder_by_ID", new[] { classCode, securityCode, TypeConverter.ToString(trans_id) });
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderByNumberAsync(long order_num)
        {
            return ExecuteCommandAsync<string, Order>("getOrder_by_Number", TypeConverter.ToString(order_num));
        }
    }
}
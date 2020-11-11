﻿// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using QuikSharp.QuikClient;
using QuikSharp.Messages;
using QuikSharp.QuikFunctions.Trading;
using System.Diagnostics;

namespace QuikSharp.QuikFunctions.Orders
{
    /// <summary>
    /// Класс, содержащий методы работы с заявками.
    /// </summary>
    public class OrderFunctions : IOrderFunctions
    {
        private readonly IQuikClient _quikClient;
        private readonly ITradingFunctions _tradingFunctions;

        public OrderFunctions(
            IQuikClient quikClient,
            ITradingFunctions tradingFunctions)
        {
            _quikClient = quikClient;
            _tradingFunctions = tradingFunctions;
        }

        /// <summary>
        /// Создание новой заявки.
        /// </summary>
        /// <param name="order">Инфомация о новой заявки, на основе которой будет сформирована транзакция.</param>
        public async Task<long> CreateOrderAsync(Order order)
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
            return await _tradingFunctions.SendTransactionAsync(newOrderTransaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Создание "лимитрированной"заявки.
        /// </summary>
        /// <param name="classCode">Код класса инструмента</param>
        /// <param name="securityCode">Код инструмента</param>
        /// <param name="accountID">Счет клиента</param>
        /// <param name="operation">Операция заявки (покупка/продажа)</param>
        /// <param name="price">Цена заявки</param>
        /// <param name="qty">Количество (в лотах)</param>
        public async Task<Order> SendLimitOrderAsync(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty)
        {
            return await SendOrder(classCode, securityCode, accountID, operation, price, qty, TransactionType.L).ConfigureAwait(false);
        }

        /// <summary>
        /// Создание "рыночной"заявки.
        /// </summary>
        /// <param name="classCode">Код класса инструмента</param>
        /// <param name="securityCode">Код инструмента</param>
        /// <param name="accountID">Счет клиента</param>
        /// <param name="operation">Операция заявки (покупка/продажа)</param>
        /// <param name="qty">Количество (в лотах)</param>
        public async Task<Order> SendMarketOrderAsync(string classCode, string securityCode, string accountID, Operation operation, int qty)
        {
            return await SendOrder(classCode, securityCode, accountID, operation, 0, qty, TransactionType.M).ConfigureAwait(false);
        }

        /// <summary>
        /// Создание заявки.
        /// </summary>
        /// <param name="classCode">Код класса инструмента</param>
        /// <param name="securityCode">Код инструмента</param>
        /// <param name="accountID">Счет клиента</param>
        /// <param name="operation">Операция заявки (покупка/продажа)</param>
        /// <param name="price">Цена заявки</param>
        /// <param name="qty">Количество (в лотах)</param>
        /// <param name="orderType">Тип заявки (L - лимитная, M - рыночная)</param>
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

        /// <summary>
        /// Отмена заявки.
        /// </summary>
        /// <param name="order">Информация по заявке, которую требуется отменить.</param>
        public async Task<long> KillOrderAsync(Order order)
        {
            Transaction killOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_ORDER,
                CLASSCODE = order.ClassCode,
                SECCODE = order.SecCode,
                ORDER_KEY = order.OrderNum.ToString()
            };
            return await _tradingFunctions.SendTransactionAsync(killOrderTransaction).ConfigureAwait(false);
        }

        /// <summary>
        /// Возвращает заявку из хранилища терминала по её номеру.
        /// На основе: http://help.qlua.org/ch4_5_1_1.htm
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="orderId">Номер заявки.</param>
        /// <returns></returns>
        public async Task<Order> GetOrderAsync(string classCode, long orderId)
        {
            var message = new Command<string>(classCode + "|" + orderId, "get_order_by_number");
            Message<Order> response = await _quikClient.SendAsync<Result<Order>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список всех заявок.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetOrdersAsync()
        {
            var message = new Command<string>("", "get_orders");
            Message<List<Order>> response = await _quikClient.SendAsync<Result<List<Order>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список заявок для заданного инструмента.
        /// </summary>
        public async Task<List<Order>> GetOrdersAsync(string classCode, string securityCode)
        {
            var message = new Command<string>(classCode + "|" + securityCode, "get_orders");
            Message<List<Order>> response = await _quikClient.SendAsync<Result<List<Order>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает заявку для заданного инструмента по ID.
        /// </summary>
        public async Task<Order> GetOrderByTransactionIdAsync(string classCode, string securityCode, long trans_id)
        {
            var message = new Command<string>(classCode + "|" + securityCode + "|" + trans_id, "getOrder_by_ID");
            Message<Order> response = await _quikClient.SendAsync<Result<Order>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает заявку по номеру.
        /// </summary>
        public async Task<Order> GetOrderByNumberAsync(long order_num)
        {
            var message = new Command<string>(order_num.ToString(), "getOrder_by_Number");
            Message<Order> response = await _quikClient.SendAsync<Result<Order>>(message).ConfigureAwait(false);
            return response.Data;
        }
    }
}
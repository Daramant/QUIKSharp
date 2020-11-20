// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Extensions;
using QuikSharp.TypeConverters;
using System;
using System.Diagnostics;

namespace QuikSharp.QuikEvents
{
    public class QuikEvents : IQuikEvents, IQuikEventsInvoker
    {
        private readonly IPersistentStorage _persistentStorage;
        private readonly ITypeConverter _typeConverter;

        public QuikEvents(
            IPersistentStorage persistentStorage,
            ITypeConverter typeConverter)
        {
            _persistentStorage = persistentStorage;
            _typeConverter = typeConverter;
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений текущей позиции по счету.
        /// </summary>
        public event AccountBalanceHandler AccountBalance;

        public void OnAccountBalance(AccountBalance accBal)
        {
            AccountBalance?.Invoke(accBal);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении денежной позиции по счету.
        /// </summary>
        public event AccountPositionHandler AccountPosition;

        public void OnAccountPosition(AccountPosition accPos)
        {
            AccountPosition?.Invoke(accPos);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении обезличенной сделки.
        /// </summary>
        public event AllTradeHandler AllTrade;

        public void OnAllTrade(AllTrade allTrade)
        {
            AllTrade?.Invoke(allTrade);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при смене сессии и при выгрузке файла qlua.dll
        /// </summary>
        public event VoidHandler CleanUp;

        public void OnCleanUp()
        {
            CleanUp?.Invoke();
        }

        /// <summary>
        /// Функция вызывается перед закрытием терминала QUIK.
        /// </summary>
        public event VoidHandler Close;

        public void OnClose()
        {
            Close?.Invoke();
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при установлении связи с сервером QUIK.
        /// </summary>
        public event VoidHandler Connected;

        public void OnConnected()
        {
            Connected?.Invoke();
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений лимита по бумагам.
        /// </summary>
        public event DepoLimitHandler DepoLimit;

        public void OnDepoLimit(DepoLimitEx dLimit)
        {
            DepoLimit?.Invoke(dLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении клиентского лимита по бумагам.
        /// </summary>
        public event DepoLimitDeleteHandler DepoLimitDelete;

        public void OnDepoLimitDelete(DepoLimitDelete dLimitDel)
        {
            DepoLimitDelete?.Invoke(dLimitDel);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при отключении от сервера QUIK.
        /// </summary>
        public event VoidHandler Disconnected;

        public void OnDisconnected()
        {
            Disconnected?.Invoke();
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении описания новой фирмы от сервера.
        /// </summary>
        public event FirmHandler Firm;

        public void OnFirm(Firm frm)
        {
            Firm?.Invoke(frm);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении позиции по срочному рынку.
        /// </summary>
        public event FuturesClientHoldingHandler FuturesClientHolding;

        public void OnFuturesClientHolding(FuturesClientHolding futPos)
        {
            FuturesClientHolding?.Invoke(futPos);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений ограничений по срочному рынку.
        /// </summary>
        public event FuturesLimitHandler FuturesLimitChange;

        public void OnFuturesLimitChange(FuturesLimits futLimit)
        {
            FuturesLimitChange?.Invoke(futLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении лимита по срочному рынку.
        /// </summary>
        public event FuturesLimitDeleteHandler FuturesLimitDelete;

        public void OnFuturesLimitDelete(FuturesLimitDelete limDel)
        {
            FuturesLimitDelete?.Invoke(limDel);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений по денежному лимиту клиента.
        /// </summary>
        public event MoneyLimitHandler MoneyLimit;

        public void OnMoneyLimit(MoneyLimitEx mLimit)
        {
            MoneyLimit?.Invoke(mLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении денежного лимита.
        /// </summary>
        public event MoneyLimitDeleteHandler MoneyLimitDelete;

        public void OnMoneyLimitDelete(MoneyLimitDelete mLimitDel)
        {
            MoneyLimitDelete?.Invoke(mLimitDel);
        }

        public event EventHandler NegDeal;

        public event EventHandler NegTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении заявки или изменении параметров существующей заявки.
        /// </summary>
        public event OrderHandler Order;

        public void OnOrder(Order order)
        {
            Order?.Invoke(order);
            // invoke event specific for the transaction
            string correlationId = _typeConverter.ToString(order.TransID);

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + order.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set<Transaction>(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnOrder(order);
                }
            }

            Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all order messages are recieved");
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при при изменении текущих параметров.
        /// </summary>
        public event ParamHandler Param;

        public void OnParam(Param par)
        {
            Param?.Invoke(par);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменения стакана котировок.
        /// </summary>
        public event QuoteHandler Quote;

        public void OnQuote(OrderBook orderBook)
        {
            Quote?.Invoke(orderBook);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления и при закрытии терминала QUIK.
        /// </summary>
        public event StopHandler Stop;

        public void OnStop(int signal)
        {
            Stop?.Invoke(signal);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой стоп-заявки или при изменении параметров существующей стоп-заявки.
        /// </summary>
        public event StopOrderHandler StopOrder;

        public void OnStopOrder(StopOrder stopOrder)
        {
            //if (OnStopOrder != null) OnStopOrder(stopOrder);
            StopOrder?.Invoke(stopOrder);
            // invoke event specific for the transaction
            string correlationId = _typeConverter.ToString(stopOrder.TransId);

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + stopOrder.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set<Transaction>(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnStopOrder(stopOrder);
                }
            }

            Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all order messages are recieved");
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки.
        /// </summary>
        public event TradeHandler Trade;

        public void OnTrade(Trade trade)
        {
            Trade?.Invoke(trade);
            // invoke event specific for the transaction
            string correlationId = trade.Comment;

            // ignore unknown transactions
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + trade.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set<Transaction>(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnTrade(trade);
                    // persist transaction with added trade
                    _persistentStorage.Set(correlationId, tr);
                }
            }

            // ignore unknown transactions
            //Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all trades messages are recieved");
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении ответа на транзакцию пользователя.
        /// </summary>
        public event TransReplyHandler TransReply;

        public void OnTransReply(TransactionReply reply)
        {
            TransReply?.Invoke(reply);

            // invoke event specific for the transaction
            if (string.IsNullOrEmpty(reply.Comment)) //"Initialization user successful" transaction doesn't contain comment
                return;

            if (_persistentStorage.Contains(reply.Comment))
            {
                var tr = _persistentStorage.Get<Transaction>(reply.Comment);
                lock (tr)
                {
                    tr.OnTransReply(reply);
                }
            }
            else
            {
                // NB ignore unmatched transactions
                //Trace.Fail("Transaction must exist in persistent storage until it is completed and its reply is recieved");
            }
        }

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        public event CandleHandler Candle;

        public void OnCandle(Candle candle)
        {
            Candle?.Invoke(candle);
        }

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        public event EventHandler<string> Error;

        public void OnError(string error)
        {
            Error?.Invoke(this, error);
        }
    }
}
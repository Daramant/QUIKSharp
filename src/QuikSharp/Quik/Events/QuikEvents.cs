// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Extensions;
using QuikSharp.Quik;
using QuikSharp.Transactions;
using QuikSharp.TypeConverters;
using System;
using System.Diagnostics;

namespace QuikSharp.QuikEvents
{
    public class QuikEvents : IQuikEvents, IQuikEventsInvoker
    {
        private IQuik _quik;

        public QuikEvents()
        { }

        public void SetEventSender(IQuik quik)
        {
            _quik = quik;
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений текущей позиции по счету.
        /// </summary>
        public event QuikEventHandler<AccountBalance> AccountBalance;

        public void OnAccountBalance(AccountBalance accBal)
        {
            AccountBalance?.Invoke(_quik, accBal);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении денежной позиции по счету.
        /// </summary>
        public event QuikEventHandler<AccountPosition> AccountPosition;

        public void OnAccountPosition(AccountPosition accPos)
        {
            AccountPosition?.Invoke(_quik, accPos);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении обезличенной сделки.
        /// </summary>
        public event QuikEventHandler<AllTrade> AllTrade;

        public void OnAllTrade(AllTrade allTrade)
        {
            AllTrade?.Invoke(_quik, allTrade);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при смене сессии и при выгрузке файла qlua.dll
        /// </summary>
        public event QuikEventHandler<EventArgs> CleanUp;

        public void OnCleanUp()
        {
            CleanUp?.Invoke(_quik, EventArgs.Empty);
        }

        /// <summary>
        /// Функция вызывается перед закрытием терминала QUIK.
        /// </summary>
        public event QuikEventHandler<EventArgs> Close;

        public void OnClose()
        {
            Close?.Invoke(_quik, EventArgs.Empty);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при установлении связи с сервером QUIK.
        /// </summary>
        public event QuikEventHandler<EventArgs> Connected;

        public void OnConnected()
        {
            Connected?.Invoke(_quik, EventArgs.Empty);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений лимита по бумагам.
        /// </summary>
        public event QuikEventHandler<DepoLimitEx> DepoLimit;

        public void OnDepoLimit(DepoLimitEx dLimit)
        {
            DepoLimit?.Invoke(_quik, dLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении клиентского лимита по бумагам.
        /// </summary>
        public event QuikEventHandler<DepoLimitDelete> DepoLimitDelete;

        public void OnDepoLimitDelete(DepoLimitDelete dLimitDel)
        {
            DepoLimitDelete?.Invoke(_quik, dLimitDel);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при отключении от сервера QUIK.
        /// </summary>
        public event QuikEventHandler<EventArgs> Disconnected;

        public void OnDisconnected()
        {
            Disconnected?.Invoke(_quik, EventArgs.Empty);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении описания новой фирмы от сервера.
        /// </summary>
        public event QuikEventHandler<Firm> Firm;

        public void OnFirm(Firm frm)
        {
            Firm?.Invoke(_quik, frm);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении позиции по срочному рынку.
        /// </summary>
        public event QuikEventHandler<FuturesClientHolding> FuturesClientHolding;

        public void OnFuturesClientHolding(FuturesClientHolding futPos)
        {
            FuturesClientHolding?.Invoke(_quik, futPos);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений ограничений по срочному рынку.
        /// </summary>
        public event QuikEventHandler<FuturesLimits> FuturesLimitChange;

        public void OnFuturesLimitChange(FuturesLimits futLimit)
        {
            FuturesLimitChange?.Invoke(_quik, futLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении лимита по срочному рынку.
        /// </summary>
        public event QuikEventHandler<FuturesLimitDelete> FuturesLimitDelete;

        public void OnFuturesLimitDelete(FuturesLimitDelete limDel)
        {
            FuturesLimitDelete?.Invoke(_quik, limDel);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений по денежному лимиту клиента.
        /// </summary>
        public event QuikEventHandler<MoneyLimitEx> MoneyLimit;

        public void OnMoneyLimit(MoneyLimitEx mLimit)
        {
            MoneyLimit?.Invoke(_quik, mLimit);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении денежного лимита.
        /// </summary>
        public event QuikEventHandler<MoneyLimitDelete> MoneyLimitDelete;

        public void OnMoneyLimitDelete(MoneyLimitDelete mLimitDel)
        {
            MoneyLimitDelete?.Invoke(_quik, mLimitDel);
        }

        public event QuikEventHandler<EventArgs> NegDeal;

        public event QuikEventHandler<EventArgs> NegTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении заявки или изменении параметров существующей заявки.
        /// </summary>
        public event QuikEventHandler<Order> Order;

        public void OnOrder(Order order)
        {
            Order?.Invoke(_quik, order);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при при изменении текущих параметров.
        /// </summary>
        public event QuikEventHandler<Param> Param;

        public void OnParam(Param par)
        {
            Param?.Invoke(_quik, par);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменения стакана котировок.
        /// </summary>
        public event QuikEventHandler<OrderBook> Quote;

        public void OnQuote(OrderBook orderBook)
        {
            Quote?.Invoke(_quik, orderBook);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления и при закрытии терминала QUIK.
        /// </summary>
        public event QuikEventHandler<StopEventArgs> Stop;

        public void OnStop(int signal)
        {
            Stop?.Invoke(_quik, new StopEventArgs(signal));
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой стоп-заявки или при изменении параметров существующей стоп-заявки.
        /// </summary>
        public event QuikEventHandler<StopOrder> StopOrder;

        public void OnStopOrder(StopOrder stopOrder)
        {
            StopOrder?.Invoke(_quik, stopOrder);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки.
        /// </summary>
        public event QuikEventHandler<Trade> Trade;

        public void OnTrade(Trade trade)
        {
            Trade?.Invoke(_quik, trade);
        }

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении ответа на транзакцию пользователя.
        /// </summary>
        public event QuikEventHandler<TransactionReply> TransReply;

        public void OnTransReply(TransactionReply reply)
        {
            TransReply?.Invoke(_quik, reply);
        }

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        public event QuikEventHandler<Candle> Candle;

        public void OnCandle(Candle candle)
        {
            Candle?.Invoke(_quik, candle);
        }

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        public event QuikEventHandler<ErrorEventArgs> Error;

        public void OnError(string error)
        {
            Error?.Invoke(_quik, new ErrorEventArgs(error));
        }
    }
}
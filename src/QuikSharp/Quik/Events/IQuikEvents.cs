// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;

namespace QuikSharp.Quik.Events
{
    // TODO Redirect these callbacks to events or rather do with events from the beginning

    /// <summary>
    /// Implements all Quik callback functions to be processed  .NET side.
    /// These functions are called by Quik inside QLUA.
    ///
    /// Функции обратного вызова
    /// Функции вызываются при получении следующих данных или событий терминалом QUIK от сервера:
    /// main - реализация основного потока исполнения в скрипте
    /// AccountBalance - изменение позиции по счету
    /// AccountPosition - изменение позиции по счету
    /// AllTrade - новая обезличенная сделка
    /// CleanUp - смена торговой сессии и при выгрузке файла qlua.dll
    /// Close - закрытие терминала QUIK
    /// Connected - установление связи с сервером QUIK
    /// DepoLimit - изменение бумажного лимита
    /// DepoLimitDelete - удаление бумажного лимита
    /// Disconnected - отключение от сервера QUIK
    /// Firm - описание новой фирмы
    /// FuturesClientHolding - изменение позиции по срочному рынку
    /// FuturesLimitChange - изменение ограничений по срочному рынку
    /// FuturesLimitDelete - удаление лимита по срочному рынку
    /// Init - инициализация функции main
    /// MoneyLimit - изменение денежного лимита
    /// MoneyLimitDelete - удаление денежного лимита
    /// NegDeal - новая заявка на внебиржевую сделку
    /// NegTrade - новая сделка для исполнения
    /// Order - новая заявка или изменение параметров существующей заявки
    /// Param - изменение текущих параметров
    /// Quote - изменение стакана котировок
    /// Stop - остановка скрипта из диалога управления
    /// StopOrder - новая стоп-заявка или изменение параметров существующей стоп-заявки
    /// Trade - новая сделка
    /// TransReply - ответ на транзакцию
    /// </summary>
    public interface IQuikEvents
    {
        /// <summary>
        /// Событие вызывается при получении изменений текущей позиции по счету.
        /// </summary>
        event QuikEventHandler<AccountBalance> AccountBalance;

        /// <summary>
        /// Событие вызывается при изменении денежной позиции по счету.
        /// </summary>
        event QuikEventHandler<AccountPosition> AccountPosition;

        /// <summary>
        /// Новая обезличенная сделка
        /// </summary>
        event QuikEventHandler<AllTrade> AllTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при смене сессии и при выгрузке файла qlua.dll
        /// </summary>
        event QuikEventHandler<EventArgs> CleanUp;

        /// <summary>
        /// Функция вызывается перед закрытием терминала QUIK.
        /// </summary>
        event QuikEventHandler<EventArgs> Close;

        /// <summary>
        /// Функция вызывается терминалом QUIK при установлении связи с сервером QUIK.
        /// </summary>
        event QuikEventHandler<EventArgs> Connected;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений лимита по бумагам.
        /// </summary>
        event QuikEventHandler<DepoLimitEx> DepoLimit;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении клиентского лимита по бумагам.
        /// </summary>
        event QuikEventHandler<DepoLimitDelete> DepoLimitDelete;

        /// <summary>
        /// Функция вызывается терминалом QUIK при отключении от сервера QUIK.
        /// </summary>
        event QuikEventHandler<EventArgs> Disconnected;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении описания новой фирмы от сервера.
        /// </summary>
        event QuikEventHandler<Firm> Firm;

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении позиции по срочному рынку.
        /// </summary>
        event QuikEventHandler<FuturesClientHolding> FuturesClientHolding;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений ограничений по срочному рынку.
        /// </summary>
        event QuikEventHandler<FuturesLimits> FuturesLimitChange;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении лимита по срочному рынку.
        /// </summary>
        event QuikEventHandler<FuturesLimitDelete> FuturesLimitDelete;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений по денежному лимиту клиента.
        /// </summary>
        event QuikEventHandler<MoneyLimitEx> MoneyLimit;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении денежного лимита.
        /// </summary>
        event QuikEventHandler<MoneyLimitDelete> MoneyLimitDelete;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении внебиржевой заявки.
        /// </summary>
        event QuikEventHandler<EventArgs> NegDeal;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки для исполнения.
        /// </summary>
        event QuikEventHandler<EventArgs> NegTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой заявки или при изменении параметров существующей заявки.
        /// </summary>
        event QuikEventHandler<Order> Order;

        /// <summary>
        /// Функция вызывается терминалом QUIK при при изменении текущих параметров.
        /// </summary>
        event QuikEventHandler<Param> Param;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменения стакана котировок.
        /// </summary>
        event QuikEventHandler<OrderBook> Quote;

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления.
        /// Примечание: Значение параметра «stop_flag» – «1».После окончания выполнения функции таймаут завершения работы скрипта 5 секунд. По истечении этого интервала функция main() завершается принудительно. При этом возможна потеря системных ресурсов.
        /// </summary>
        event QuikEventHandler<StopEventArgs> Stop;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой стоп-заявки или при изменении параметров существующей стоп-заявки.
        /// </summary>
        event QuikEventHandler<StopOrder> StopOrder;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки.
        /// </summary>
        event QuikEventHandler<Trade> Trade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении ответа на транзакцию пользователя.
        /// </summary>
        event QuikEventHandler<TransactionReply> TransReply;

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        event QuikEventHandler<Candle> Candle;

        event QuikEventHandler<ErrorEventArgs> Error;
    }
}
// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;

namespace QuikSharp.QuikEvents
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
        /// Событие вызывается когда библиотека QuikSharp успешно подключилась к Quik'у
        /// </summary>
        event InitHandler ConnectedToQuik;

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp была отключена от Quik'а
        /// </summary>
        event VoidHandler DisconnectedFromQuik;

        /// <summary>
        /// Событие вызывается при получении изменений текущей позиции по счету.
        /// </summary>
        event AccountBalanceHandler AccountBalance;

        /// <summary>
        /// Событие вызывается при изменении денежной позиции по счету.
        /// </summary>
        event AccountPositionHandler AccountPosition;

        /// <summary>
        /// Новая обезличенная сделка
        /// </summary>
        event AllTradeHandler AllTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при смене сессии и при выгрузке файла qlua.dll
        /// </summary>
        event VoidHandler CleanUp;

        /// <summary>
        /// Функция вызывается перед закрытием терминала QUIK.
        /// </summary>
        event VoidHandler Close;

        /// <summary>
        /// Функция вызывается терминалом QUIK при установлении связи с сервером QUIK.
        /// </summary>
        event VoidHandler Connected;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений лимита по бумагам.
        /// </summary>
        event DepoLimitHandler DepoLimit;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении клиентского лимита по бумагам.
        /// </summary>
        event DepoLimitDeleteHandler DepoLimitDelete;

        /// <summary>
        /// Функция вызывается терминалом QUIK при отключении от сервера QUIK.
        /// </summary>
        event VoidHandler Disconnected;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении описания новой фирмы от сервера.
        /// </summary>
        event FirmHandler Firm;

        /// <summary>
        /// Функция вызывается терминалом QUIK при изменении позиции по срочному рынку.
        /// </summary>
        event FuturesClientHoldingHandler FuturesClientHolding;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений ограничений по срочному рынку.
        /// </summary>
        event FuturesLimitHandler FuturesLimitChange;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении лимита по срочному рынку.
        /// </summary>
        event FuturesLimitDeleteHandler FuturesLimitDelete;

        /// <summary>
        /// Depricated
        /// </summary>
        event InitHandler Init;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменений по денежному лимиту клиента.
        /// </summary>
        event MoneyLimitHandler MoneyLimit;

        /// <summary>
        /// Функция вызывается терминалом QUIK при удалении денежного лимита.
        /// </summary>
        event MoneyLimitDeleteHandler MoneyLimitDelete;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении внебиржевой заявки.
        /// </summary>
        event EventHandler NegDeal;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки для исполнения.
        /// </summary>
        event EventHandler NegTrade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой заявки или при изменении параметров существующей заявки.
        /// </summary>
        event OrderHandler Order;

        /// <summary>
        /// Функция вызывается терминалом QUIK при при изменении текущих параметров.
        /// </summary>
        event ParamHandler Param;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении изменения стакана котировок.
        /// </summary>
        event QuoteHandler Quote;

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления.
        /// Примечание: Значение параметра «stop_flag» – «1».После окончания выполнения функции таймаут завершения работы скрипта 5 секунд. По истечении этого интервала функция main() завершается принудительно. При этом возможна потеря системных ресурсов.
        /// </summary>
        event StopHandler Stop;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении новой стоп-заявки или при изменении параметров существующей стоп-заявки.
        /// </summary>
        event StopOrderHandler StopOrder;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении сделки.
        /// </summary>
        event TradeHandler Trade;

        /// <summary>
        /// Функция вызывается терминалом QUIK при получении ответа на транзакцию пользователя.
        /// </summary>
        event TransReplyHandler TransReply;

        /// <summary>
        /// Событие получения новой свечи. Для срабатывания необходимо подписаться с помощью метода Subscribe.
        /// </summary>
        event CandleHandler NewCandle;
    }
}
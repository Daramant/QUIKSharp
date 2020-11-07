﻿// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Candles
{
    /// <summary>
    /// Функции для получения свечей
    /// </summary>
    public class CandleFunctions : ICandleFunctions
    {
        private readonly IQuikService _quikService;

        public CandleFunctions(IQuikService quikService)
        {
            _quikService = quikService;
        }

        /// <summary>
        /// Функция предназначена для получения количества свечей по тегу
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        public async Task<int> GetNumCandlesAsync(string graphicTag)
        {
            var message = new Command<string>(graphicTag, "get_num_candles");
            Message<int> response = await _quikService.SendAsync<Result<int>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция предназначена для получения информации о свечках по идентификатору (заказ данных для построения графика плагин не осуществляет, поэтому для успешного доступа нужный график должен быть открыт). Возвращаются все доступные свечки.
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        public async Task<List<Candle>> GetAllCandlesAsync(string graphicTag)
        {
            return await GetCandlesAsync(graphicTag, 0, 0, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Функция предназначена для получения информации о свечках по идентификатору (заказ данных для построения графика плагин не осуществляет, поэтому для успешного доступа нужный график должен быть открыт).
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <param name="line">Номер линии графика или индикатора. Первая линия имеет номер 0</param>
        /// <param name="first">Индекс первой свечки. Первая (самая левая) свечка имеет индекс 0</param>
        /// <param name="count">Количество запрашиваемых свечек</param>
        /// <returns></returns>
        public async Task<List<Candle>> GetCandlesAsync(string graphicTag, int line, int first, int count)
        {
            var message = new Command<string>(graphicTag + "|" + line + "|" + first + "|" + count, "get_candles");
            var response = await _quikService.SendAsync<Result<List<Candle>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция возвращает список свечек указанного инструмента заданного интервала.
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">Интервал свечей.</param>
        /// <returns>Список свечей.</returns>
        public async Task<List<Candle>> GetAllCandlesAsync(string classCode, string securityCode, CandleInterval interval)
        {
            //Параметр count == 0 говорт о том, что возвращаются все доступные свечи
            return await GetLastCandlesAsync(classCode, securityCode, interval, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Возвращает заданное количество свечек указанного инструмента и интервала с конца.
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">Интервал свечей.</param>
        /// <param name="count">Количество возвращаемых свечей с конца.</param>
        /// <returns>Список свечей.</returns>
        public async Task<List<Candle>> GetLastCandlesAsync(string classCode, string securityCode, CandleInterval interval, int count)
        {
            var message = new Command<string>(classCode + "|" + securityCode + "|" + (int) interval + "|" + count, "get_candles_from_data_source");
            IResult<List<Candle>> response = await _quikService.SendAsync<Result<List<Candle>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Осуществляет подписку на получение исторических данных (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public async Task SubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string>(classCode + "|" + securityCode + "|" + (int) interval, "subscribe_to_candles");
            await _quikService.SendAsync<Result<string>>(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Отписывается от получения исторических данных (свечей)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public async Task UnsubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string>(classCode + "|" + securityCode + "|" + (int) interval, "unsubscribe_from_candles");
            await _quikService.SendAsync<Result<string>>(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Проверка состояния подписки на исторические данные (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public async Task<bool> IsSubscribedAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string>(classCode + "|" + securityCode + "|" + (int) interval, "is_subscribed");
            Message<bool> response = await _quikService.SendAsync<Result<bool>>(message).ConfigureAwait(false);
            return response.Data;
        }
    }
}
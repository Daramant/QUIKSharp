// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Extensions;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Candles
{
    /// <summary>
    /// Функции для получения свечей
    /// </summary>
    public class CandleFunctions : ICandleFunctions
    {
        private readonly IQuikClient _quikClient;

        public CandleFunctions(IQuikClient quikClient)
        {
            _quikClient = quikClient;
        }

        /// <summary>
        /// Функция предназначена для получения количества свечей по тегу
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        public async Task<int> GetNumCandlesAsync(string graphicTag)
        {
            var message = new Command<string>(graphicTag, "get_num_candles");
            var response = await _quikClient.SendAsync<Result<int>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция предназначена для получения информации о свечках по идентификатору (заказ данных для построения графика плагин не осуществляет, поэтому для успешного доступа нужный график должен быть открыт). Возвращаются все доступные свечки.
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        public Task<List<Candle>> GetAllCandlesAsync(string graphicTag)
        {
            return GetCandlesAsync(graphicTag, 0, 0, 0);
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
            var message = new Command<string[]>(new[] { graphicTag, line.ToQuikStringLookup(), first.ToQuikStringLookup(), count.ToQuikStringLookup() }, "get_candles");
            var response = await _quikClient.SendAsync<Result<List<Candle>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция возвращает список свечек указанного инструмента заданного интервала.
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">Интервал свечей.</param>
        /// <returns>Список свечей.</returns>
        public Task<List<Candle>> GetAllCandlesAsync(string classCode, string securityCode, CandleInterval interval)
        {
            //Параметр count == 0 говорт о том, что возвращаются все доступные свечи
            return GetLastCandlesAsync(classCode, securityCode, interval, 0);
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
            var message = new Command<string[]>(new[] { classCode, securityCode, interval.ToQuikIntStringLookup(), count.ToQuikStringLookup() }, "get_candles_from_data_source");
            var response = await _quikClient.SendAsync<Result<List<Candle>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Осуществляет подписку на получение исторических данных (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public Task SubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string[]>(new[] { classCode, securityCode, interval.ToQuikIntStringLookup() }, "subscribe_to_candles");
            return _quikClient.SendAsync<Result<string>>(message);
        }

        /// <summary>
        /// Отписывается от получения исторических данных (свечей)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public Task UnsubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string[]>(new[] { classCode, securityCode, interval.ToQuikIntStringLookup() }, "unsubscribe_from_candles");
            return _quikClient.SendAsync<Result<string>>(message);
        }

        /// <summary>
        /// Проверка состояния подписки на исторические данные (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        public async Task<bool> IsSubscribedAsync(string classCode, string securityCode, CandleInterval interval)
        {
            var message = new Command<string[]>(new[] { classCode, securityCode, interval.ToQuikIntStringLookup() }, "is_subscribed");
            var response = await _quikClient.SendAsync<Result<bool>>(message).ConfigureAwait(false);
            return response.Data;
        }
    }
}
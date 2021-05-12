using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Candles
{
    /// <summary>
    /// Функции для работы с графиками.
    /// </summary>
    public interface ICandleFunctions
    {
        /// <summary>
        /// Функция предназначена для получения количества свечей по тегу
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        Task<int> GetNumCandlesAsync(string graphicTag);

        /// <summary>
        /// Функция предназначена для получения информации о свечках по идентификатору (заказ данных для построения графика плагин не осуществляет, поэтому для успешного доступа нужный график должен быть открыт). Возвращаются все доступные свечки.
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Candle>> GetAllCandlesAsync(string graphicTag);

        /// <summary>
        /// Функция предназначена для получения информации о свечках по идентификатору (заказ данных для построения графика плагин не осуществляет, поэтому для успешного доступа нужный график должен быть открыт).
        /// </summary>
        /// <param name="graphicTag">Строковый идентификатор графика или индикатора</param>
        /// <param name="line">Номер линии графика или индикатора. Первая линия имеет номер 0</param>
        /// <param name="first">Индекс первой свечки. Первая (самая левая) свечка имеет индекс 0</param>
        /// <param name="count">Количество запрашиваемых свечек</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Candle>> GetCandlesAsync(string graphicTag, int line, int first, int count);

        /// <summary>
        /// Функция возвращает список свечек указанного инструмента заданного интервала.
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">Интервал свечей.</param>
        /// <returns>Список свечей.</returns>
        Task<IReadOnlyCollection<Candle>> GetAllCandlesAsync(string classCode, string securityCode, CandleInterval interval);

        /// <summary>
        /// Возвращает заданное количество свечек указанного инструмента и интервала с конца.
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">Интервал свечей.</param>
        /// <param name="count">Количество возвращаемых свечей с конца.</param>
        /// <returns>Список свечей.</returns>
        Task<IReadOnlyCollection<Candle>> GetCandlesFromDataSourceAsync(string classCode, string securityCode, CandleInterval interval, int count);

        /// <summary>
        /// Осуществляет подписку на получение исторических данных (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        Task SubscribeAsync(string classCode, string securityCode, CandleInterval interval);

        /// <summary>
        /// Отписывается от получения исторических данных (свечей)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        Task UnsubscribeAsync(string classCode, string securityCode, CandleInterval interval);

        /// <summary>
        /// Проверка состояния подписки на исторические данные (свечи)
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="securityCode">Код инструмента.</param>
        /// <param name="interval">интервал свечей (тайм-фрейм).</param>
        Task<bool> IsSubscribedAsync(string classCode, string securityCode, CandleInterval interval);
    }
}

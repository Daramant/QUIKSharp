using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.TableRows
{
    public interface ITableRowFunctions
    {
        /// <summary>
        /// Функция возвращает код клиента.
        /// </summary>
        Task<string> GetClientCodeAsync();

        /// <summary>
        /// Функция возвращает таблицу с описанием торгового счета для запрашиваемого кода класса.
        /// </summary>
        Task<string> GetTradeAccountAsync(string classCode);

        /// <summary>
        /// Функция возвращает таблицу всех счетов в торговой системе.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<TradesAccounts>> GetTradeAccountsAsync();

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        Task<IReadOnlyCollection<MoneyLimitEx>> GetMoneyLimitsAsync();

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        Task<IReadOnlyCollection<FuturesLimits>> GetFuturesClientLimitsAsync();

        /// <summary>
        /// Возвращает заявку из хранилища терминала по её номеру.
        /// На основе: http://help.qlua.org/ch4_5_1_1.htm
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="orderId">Номер заявки.</param>
        /// <returns></returns>
        Task<Order> GetOrderByNumberAsync(string classCode, long orderId);

        /// <summary>
        /// Возвращает список всех заявок.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<Order>> GetOrdersAsync();

        /// <summary>
        /// Возвращает список заявок для заданного инструмента.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="securityCode"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Order>> GetOrdersAsync(string classCode, string securityCode);

        /// <summary>
        /// Возвращает заявку для заданного инструмента по id транзакции.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="securityCode"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        Task<Order> GetOrderByTransactionIdAsync(string classCode, string securityCode, long transactionId);

        /// <summary>
        /// Возвращает заявку по номеру.
        /// </summary>
        /// <param name="orderNumber">Номер заявки в торговой системе.</param>
        /// <returns></returns>
        Task<Order> GetOrderByOrderNumberAsync(long orderNumber);

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync();

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        /// <param name="secCode">Код инструмента.</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync(string secCode);

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        Task<IReadOnlyCollection<Trade>> GetTradesAsync();

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Trade>> GetTradesAsync(string classCode, string secCode);

        /// <summary>
        /// функция для получения таблицы сделок номеру заявки.
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Trade>> GetTradesByOrderNumberAsync(long orderNumber);

        /// <summary>
        /// Возвращает список всех стоп-заявок.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync();

        /// <summary>
        /// Возвращает список стоп-заявок для заданного инструмента.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="securityCode"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync(string classCode, string securityCode);
    }
}

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Trading
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK.
    /// </summary>
    public interface ITradingFunctions
    {
        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync();

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        /// <param name="secCode">Код инструментаю</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync(string secCode);

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
        /// Функция получения доски опционов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode);

        

        

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
        /// функция для получения таблицы сделок номеру заявки
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<Trade>> GetTradesByOrderNumberAsync(long orderNum);

        ///// <summary>
        /////  функция для получения информации по инструменту
        ///// </summary>
        //Task<string> getSecurityInfo();
        ///// <summary>
        /////  функция для получения даты торговой сессии
        ///// </summary>
        //Task<string> getTradeDate();

        /// <summary>
        /// Функция отправляет транзакцию на сервер QUIK и сохраняет ее в словаре транзакций
        /// с идентификатором trans_id. Возвращает идентификатор
        /// транзакции trans_id (позитивное число) в случае успеха или индентификатор,
        /// умноженный на -1 (-trans_id) (негативное число) в случае ошибки. Также в случае
        /// ошибки функция созраняет текст ошибки в свойтво ErrorMessage транзакции.
        /// </summary>
        Task<long> SendTransactionAsync(Transaction transaction);

        

        ///// <summary>
        /////  функция для получения параметров таблицы «Купить/Продать»
        ///// </summary>
        //Task<string> getBuySellInfo();
        ///// <summary>
        /////  функция для получения параметров (включая вид лимита) таблицы «Купить/Продать»
        ///// </summary>
        //Task<string> getBuySellInfoEx();

        
    }
}

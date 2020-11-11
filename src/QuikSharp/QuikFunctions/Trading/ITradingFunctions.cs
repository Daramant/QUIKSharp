using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Trading
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK.
    /// </summary>
    public interface ITradingFunctions
    {
        /// <summary>
        /// Функция для получения информации по бумажным лимитам
        /// </summary>
        Task<DepoLimit> GetDepoAsync(string clientCode, string firmId, string secCode, string account);

        /// <summary>
        /// Функция для получения информации по бумажным лимитам указанного типа
        /// </summary>
        Task<DepoLimitEx> GetDepoExAsync(string firmId, string clientCode, string secCode, string accID, int limitKind);

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        Task<List<DepoLimitEx>> GetDepoLimitsAsync();

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        /// <param name="secCode">Код инструментаю</param>
        /// <returns></returns>
        Task<List<DepoLimitEx>> GetDepoLimitsAsync(string secCode);

        /// <summary>
        /// Функция для получения информации по денежным лимитам
        /// </summary>
        Task<MoneyLimit> GetMoneyAsync(string clientCode, string firmId, string tag, string currCode);

        /// <summary>
        ///  функция для получения информации по денежным лимитам указанного типа
        /// </summary>
        Task<MoneyLimitEx> GetMoneyExAsync(string firmId, string clientCode, string tag, string currCode, int limitKind);

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        Task<List<MoneyLimitEx>> GetMoneyLimitsAsync();

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам
        /// </summary>
        Task<FuturesLimits> GetFuturesLimitAsync(string firmId, string accId, int limitType, string currCode);

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        Task<List<FuturesLimits>> GetFuturesClientLimitsAsync();

        /// <summary>
        ///  функция для получения информации по фьючерсным позициям
        /// </summary>
        Task<FuturesClientHolding> GetFuturesHoldingAsync(string firmId, string accId, string secCode, int posType);

        /// <summary>
        /// Функция получения доски опционов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<List<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode);

        /// <summary>
        /// Функция заказывает получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<bool> ParamRequestAsync(string classCode, string secCode, string paramName);

        Task<bool> ParamRequestAsync(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// Функция отменяет заказ на получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<bool> CancelParamRequestAsync(string classCode, string secCode, string paramName);

        Task<bool> CancelParamRequestAsync(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// Функция для получения значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamExAsync(string classCode, string secCode, string paramName, int timeout = Timeout.Infinite);

        Task<ParamTable> GetParamExAsync(string classCode, string secCode, ParamName paramName, int timeout = Timeout.Infinite);

        /// <summary>
        /// Функция для получения всех значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamEx2Async(string classCode, string secCode, string paramName);

        Task<ParamTable> GetParamEx2Async(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        Task<List<Trade>> GetTradesAsync();

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<List<Trade>> GetTradesAsync(string classCode, string secCode);

        /// <summary>
        /// функция для получения таблицы сделок номеру заявки
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        Task<List<Trade>> GetTradesByOrderNumberAsync(long orderNum);

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
        /////  функция для расчета максимально возможного количества лотов в заявке
        ///// </summary>
        //Task<string> CulcBuySell();
        /// <summary>
        ///  функция для получения значений параметров таблицы «Клиентский портфель»
        /// </summary>
        Task<PortfolioInfo> GetPortfolioInfoAsync(string firmId, string clientCode);

        /// <summary>
        ///  функция для получения значений параметров таблицы «Клиентский портфель» с учетом вида лимита
        ///  Для получения значений параметров таблицы «Клиентский портфель» для клиентов срочного рынка без единой денежной позиции
        ///  необходимо указать в качестве «clientCode» – торговый счет на срочном рынке, а в качестве «limitKind» – 0.
        /// </summary>
        Task<PortfolioInfoEx> GetPortfolioInfoExAsync(string firmId, string clientCode, int limitKind);

        ///// <summary>
        /////  функция для получения параметров таблицы «Купить/Продать»
        ///// </summary>
        //Task<string> getBuySellInfo();
        ///// <summary>
        /////  функция для получения параметров (включая вид лимита) таблицы «Купить/Продать»
        ///// </summary>
        //Task<string> getBuySellInfoEx();

        /// <summary>
        /// Функция возвращает торговый счет срочного рынка, соответствующий коду клиента фондового рынка с единой денежной позицией
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        Task<string> GetTrdAccByClientCodeAsync(string firmId, string clientCode);

        /// <summary>
        /// Функция возвращает код клиента фондового рынка с единой денежной позицией, соответствующий торговому счету срочного рынка
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="trdAccId"></param>
        /// <returns></returns>
        Task<string> GetClientCodeByTrdAccAsync(string firmId, string trdAccId);

        /// <summary>
        /// Функция предназначена для получения признака, указывающего имеет ли клиент единую денежную позицию
        /// </summary>
        /// <param name="firmId">идентификатор фирмы фондового рынка</param>
        /// <param name="client">код клиента фондового рынка или торговый счет срочного рынка</param>
        /// <returns></returns>
        Task<bool> IsUcpClientAsync(string firmId, string client);
    }
}

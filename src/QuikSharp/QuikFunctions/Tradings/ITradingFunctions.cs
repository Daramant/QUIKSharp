using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuickFunctions.Tradings
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
    /// getDepo - функция для получения информации по бумажным лимитам
    /// getMoney - функция для получения информации по денежным лимитам
    /// getMoneyEx - функция для получения информации по денежным лимитам указанного типа
    /// getFuturesLimit - функция для получения информации по фьючерсным лимитам
    /// getFuturesHolding - функция для получения информации по фьючерсным позициям
    /// paramRequest - Функция заказывает получение параметров Таблицы текущих торгов
    /// cancelParamRequest - Функция отменяет заказ на получение параметров Таблицы текущих торгов
    /// getParamEx - функция для получения значений Таблицы текущих значений параметров
    /// getParamEx2 - функция для получения всех значений Таблицы текущих значений параметров
    /// getTradeDate - функция для получения даты торговой сессии
    /// sendTransaction - функция для работы с заявками
    /// CulcBuySell - функция для расчета максимально возможного количества лотов в заявке
    /// getPortfolioInfo - функция для получения значений параметров таблицы «Клиентский портфель»
    /// getPortfolioInfoEx - функция для получения значений параметров таблицы «Клиентский портфель» с учетом вида лимита
    /// getBuySellInfo - функция для получения параметров таблицы «Купить/Продать»
    /// getBuySellInfoEx - функция для получения параметров (включая вид лимита) таблицы «Купить/Продать»
    /// getTrdAccByClientCode - Функция возвращает торговый счет срочного рынка, соответствующий коду клиента фондового рынка с единой денежной позицией
    /// getClientCodeByTrdAcc - Функция возвращает код клиента фондового рынка с единой денежной позицией, соответствующий торговому счету срочного рынка
    /// isUcpClient - Функция предназначена для получения признака, указывающего имеет ли клиент единую денежную позицию
    /// </summary>
    public interface ITradingFunctions
    {
        /// <summary>
        /// Функция для получения информации по бумажным лимитам
        /// </summary>
        Task<DepoLimit> GetDepo(string clientCode, string firmId, string secCode, string account);

        /// <summary>
        /// Функция для получения информации по бумажным лимитам указанного типа
        /// </summary>
        Task<DepoLimitEx> GetDepoEx(string firmId, string clientCode, string secCode, string accID, int limitKind);

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        Task<List<DepoLimitEx>> GetDepoLimits();

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        /// <param name="secCode">Код инструментаю</param>
        /// <returns></returns>
        Task<List<DepoLimitEx>> GetDepoLimits(string secCode);

        /// <summary>
        /// Функция для получения информации по денежным лимитам
        /// </summary>
        ///
        Task<MoneyLimit> GetMoney(string clientCode, string firmId, string tag, string currCode);

        /// <summary>
        ///  функция для получения информации по денежным лимитам указанного типа
        /// </summary>
        Task<MoneyLimitEx> GetMoneyEx(string firmId, string clientCode, string tag, string currCode, int limitKind);

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        Task<List<MoneyLimitEx>> GetMoneyLimits();

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам
        /// </summary>
        Task<FuturesLimits> GetFuturesLimit(string firmId, string accId, int limitType, string currCode);

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        Task<List<FuturesLimits>> GetFuturesClientLimits();

        /// <summary>
        ///  функция для получения информации по фьючерсным позициям
        /// </summary>
        Task<FuturesClientHolding> GetFuturesHolding(string firmId, string accId, string secCode, int posType);

        /// <summary>
        /// Функция получения доски опционов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<List<OptionBoard>> GetOptionBoard(string classCode, string secCode);

        /// <summary>
        /// Функция заказывает получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<bool> ParamRequest(string classCode, string secCode, string paramName);

        Task<bool> ParamRequest(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// Функция отменяет заказ на получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<bool> CancelParamRequest(string classCode, string secCode, string paramName);

        Task<bool> CancelParamRequest(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// Функция для получения значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamEx(string classCode, string secCode, string paramName, int timeout = Timeout.Infinite);

        Task<ParamTable> GetParamEx(string classCode, string secCode, ParamName paramName, int timeout = Timeout.Infinite);

        /// <summary>
        /// Функция для получения всех значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamEx2(string classCode, string secCode, string paramName);

        Task<ParamTable> GetParamEx2(string classCode, string secCode, ParamName paramName);

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        Task<List<Trade>> GetTrades();

        /// <summary>
        /// функция для получения таблицы сделок по заданному инструменту
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<List<Trade>> GetTrades(string classCode, string secCode);

        /// <summary>
        /// функция для получения таблицы сделок номеру заявки
        /// </summary>
        /// <param name="orderNum"></param>
        /// <returns></returns>
        Task<List<Trade>> GetTrades_by_OdrerNumber(long orderNum);

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
        Task<long> SendTransaction(Transaction transaction);

        ///// <summary>
        /////  функция для расчета максимально возможного количества лотов в заявке
        ///// </summary>
        //Task<string> CulcBuySell();
        /// <summary>
        ///  функция для получения значений параметров таблицы «Клиентский портфель»
        /// </summary>
        Task<PortfolioInfo> GetPortfolioInfo(string firmId, string clientCode);

        /// <summary>
        ///  функция для получения значений параметров таблицы «Клиентский портфель» с учетом вида лимита
        ///  Для получения значений параметров таблицы «Клиентский портфель» для клиентов срочного рынка без единой денежной позиции
        ///  необходимо указать в качестве «clientCode» – торговый счет на срочном рынке, а в качестве «limitKind» – 0.
        /// </summary>
        Task<PortfolioInfoEx> GetPortfolioInfoEx(string firmId, string clientCode, int limitKind);

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
        Task<string> GetTrdAccByClientCode(string firmId, string clientCode);

        /// <summary>
        /// Функция возвращает код клиента фондового рынка с единой денежной позицией, соответствующий торговому счету срочного рынка
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="trdAccId"></param>
        /// <returns></returns>
        Task<string> GetClientCodeByTrdAcc(string firmId, string trdAccId);

        /// <summary>
        /// Функция предназначена для получения признака, указывающего имеет ли клиент единую денежную позицию
        /// </summary>
        /// <param name="firmId">идентификатор фирмы фондового рынка</param>
        /// <param name="client">код клиента фондового рынка или торговый счет срочного рынка</param>
        /// <returns></returns>
        Task<bool> IsUcpClient(string firmId, string client);
    }
}

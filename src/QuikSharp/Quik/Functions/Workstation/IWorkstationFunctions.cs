using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Workplace
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK.
    /// </summary>
    public interface IWorkstationFunctions
    {
        /// <summary>
        /// Функция предназначена для получения информации по денежным позициям. 
        /// </summary>
        /// <param name="clientCode"></param>
        /// <param name="firmId"></param>
        /// <param name="tag"></param>
        /// <param name="currCode"></param>
        /// <returns></returns>
        Task<MoneyLimit> GetMoneyAsync(string clientCode, string firmId, string tag, string currCode);

        /// <summary>
        /// Функция предназначена для получения информации по денежным позициям указанного типа. 
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="clientCode"></param>
        /// <param name="tag"></param>
        /// <param name="currCode"></param>
        /// <param name="limitKind"></param>
        /// <returns></returns>
        Task<MoneyLimitEx> GetMoneyExAsync(string firmId, string clientCode, string tag, string currCode, int limitKind);

        /// <summary>
        /// Функция предназначена для получения позиций по инструментам. 
        /// </summary>
        /// <param name="clientCode"></param>
        /// <param name="firmId"></param>
        /// <param name="secCode"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<DepoLimit> GetDepoAsync(string clientCode, string firmId, string secCode, string account);

        /// <summary>
        /// Функция предназначена для получения позиций по инструментам указанного типа. 
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="clientCode"></param>
        /// <param name="secCode"></param>
        /// <param name="trdAccId"></param>
        /// <param name="limitKind"></param>
        /// <returns></returns>
        Task<DepoLimitEx> GetDepoExAsync(string firmId, string clientCode, string secCode, string trdAccId, int limitKind);

        /// <summary>
        /// Функция предназначена для получения информации по фьючерсным лимитам.
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="accId"></param>
        /// <param name="limitType"></param>
        /// <param name="currCode"></param>
        /// <returns></returns>
        Task<FuturesLimits> GetFuturesLimitAsync(string firmId, string accId, int limitType, string currCode);

        /// <summary>
        /// Функция предназначена для получения информации по фьючерсным позициям.
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="accId"></param>
        /// <param name="secCode"></param>
        /// <param name="posType"></param>
        /// <returns></returns>
        Task<FuturesClientHolding> GetFuturesHoldingAsync(string firmId, string accId, string secCode, int posType);

        /// <summary>
        /// Функция предназначена для получения информации по инструменту.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<SecurityInfo> GetSecurityInfoAsync(string classCode, string secCode);

        /// <summary>
        /// Функция предназначена для определения класса по коду инструмента из заданного списка классов.
        /// </summary>
        /// <param name="classesList"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<string> GetSecurityClassAsync(string classesList, string secCode);

        /// <summary>
        /// Функция предназначена для получения стакана по указанному классу и инструменту.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<OrderBook> GetQuoteLevel2Async(string classCode, string secCode);

        /// <summary>
        /// Функция предназначена для получения значений всех параметров биржевой информации из таблицы «Текущие торги». 
        /// С помощью этой функции можно получить любое из значений Таблицы текущих торгов для заданных кодов класса и инструмента. 
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamExAsync(string classCode, string secCode, ParamName paramName, TimeSpan? timeout = null);

        /// <summary>
        /// Функция предназначена для получения значений всех параметров биржевой информации из Таблицы текущих торгов с 
        /// возможностью в дальнейшем отказаться от получения определенных параметров, заказанных с помощью функции ParamRequest. 
        /// Для отказа от получения какого-либо параметра воспользуйтесь функцией CancelParamRequest.
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<ParamTable> GetParamEx2Async(string classCode, string secCode, ParamName paramName, TimeSpan? timeout = null);

        /// <summary>
        /// Функция предназначена для отправки транзакций в торговую систему.
        /// </summary>
        /// <param name="transaction">Транзакция.</param>
        /// <param name="timeout"></param>
        /// <returns>Строка, содержащая текст ошибки, если она случилась при обработке транзакции.</returns>
        Task<string> SendTransactionAsync(Transaction transaction, TimeSpan? timeout = null);

        /// <summary>
        /// Функция предназначена для получения значений параметров таблицы «Клиентский портфель», соответствующих 
        /// идентификатору участника торгов «firmid» и коду клиента «client_code».
        /// </summary>
        Task<PortfolioInfo> GetPortfolioInfoAsync(string firmId, string clientCode);

        /// <summary>
        /// Функция предназначена для получения значений параметров таблицы «Клиентский портфель», соответствующих 
        /// идентификатору участника торгов «firmId», коду клиента «clientCode» и сроку расчётов «limitKind». 
        /// </summary>
        /// <remarks>
        /// Для получения значений параметров таблицы «Клиентский портфель» для клиентов срочного рынка без единой 
        /// денежной позиции необходимо указать в качестве «clientCode» – торговый счет на срочном рынке, а в 
        /// качестве «limitKind» – 0.
        /// </remarks>
        Task<PortfolioInfoEx> GetPortfolioInfoExAsync(string firmId, string clientCode, int limitKind);

        /// <summary>
        /// Функция возвращает торговый счет срочного рынка, соответствующий коду клиента фондового рынка с единой денежной позицией.
        /// </summary>
        /// <param name="firmId">Идентификатор фирмы фондового рынка.</param>
        /// <param name="clientCode">Код клиента.</param>
        /// <returns>Cтрока с торговым счетом срочного рынка, если указанный код клиента фондового рынка имеет единую 
        /// денежную позицию, иначе – <c>null</c>.</returns>
        Task<string> GetTrdAccByClientCodeAsync(string firmId, string clientCode);

        /// <summary>
        /// Функция возвращает код клиента фондового рынка с единой денежной позицией, соответствующий торговому счету срочного рынка.
        /// </summary>
        /// <param name="firmId">Идентификатор фирмы фондового рынка.</param>
        /// <param name="trdAccId">Торговый счет срочного рынка.</param>
        /// <returns>Строка с кодом клиента, если указанный торговый счет имеет единую денежную позицию, иначе – <c>null</c>.</returns>
        Task<string> GetClientCodeByTrdAccAsync(string firmId, string trdAccId);

        /// <summary>
        /// Функция предназначена для получения признака, указывающего имеет ли клиент единую денежную позицию.
        /// </summary>
        /// <param name="firmId">Идентификатор фирмы фондового рынка.</param>
        /// <param name="client">Код клиента фондового рынка или торговый счет срочного рынка.</param>
        /// <returns><c>True</c>, если указанный клиент имеет единую денежную позицию, иначе – <c>false</c>.</returns>
        Task<bool> IsUcpClientAsync(string firmId, string client);
    }
}

using QuikSharp;
using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Classes
{
    /// <summary>
    /// Функции для обращения к спискам доступных параметров
    /// </summary>
    public interface IClassFunctions
    {
        /// <summary>
        /// Функция предназначена для получения списка кодов классов, переданных с сервера в ходе сеанса связи.
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetClassesListAsync();

        /// <summary>
        /// Функция предназначена для получения информации о классе.
        /// </summary>
        /// <param name="classID"></param>
        Task<ClassInfo> GetClassInfoAsync(string classID);

        /// <summary>
        /// Функция предназначена для получения информации по бумаге.
        /// </summary>
        Task<SecurityInfo> GetSecurityInfoAsync(string classCode, string secCode);

        /// <summary>
        /// Функция предназначена для получения информации по бумаге.
        /// </summary>
        Task<SecurityInfo> GetSecurityInfoAsync(ISecurity security);

        /// <summary>
        /// Функция предназначена для получения списка кодов бумаг для списка классов, заданного списком кодов.
        /// </summary>
        Task<string[]> GetClassSecuritiesAsync(string classID);

        /// <summary>
        /// Функция предназначена для определения класса по коду инструмента из заданного списка классов.
        /// </summary>
        Task<string> GetSecurityClassAsync(string classesList, string secCode);

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
        Task<List<TradesAccounts>> GetTradeAccountsAsync();
    }
}

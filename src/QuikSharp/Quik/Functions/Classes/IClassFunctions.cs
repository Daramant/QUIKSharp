using QuikSharp;
using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Classes
{
    /// <summary>
    /// Функции для обращения к спискам доступных параметров
    /// </summary>
    public interface IClassFunctions
    {
        /// <summary>
        /// Функция предназначена для получения списка кодов классов, переданных с сервера в ходе сеанса связи.
        /// </summary>
        /// <returns>Список кодов классов.</returns>
        Task<string[]> GetClassesListAsync();

        /// <summary>
        /// Функция предназначена для получения информации о классе.
        /// </summary>
        /// <param name="classCode">Код класса.</param>
        /// <returns>Описание класса.</returns>
        Task<ClassInfo> GetClassInfoAsync(string classCode);

        /// <summary>
        /// Функция предназначена для получения списка кодов инструментов для списка классов, заданного списком кодов.
        /// </summary>
        /// <param name="classCode">Код класса.</param>
        /// <returns>Список кодов инструментов.</returns>
        Task<string[]> GetClassSecuritiesAsync(string classCode);
    }
}

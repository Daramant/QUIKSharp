using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Custom
{
    /// <summary>
    /// Дополнительные пользовательские функции.
    /// </summary>
    public interface ICustomFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        IMessageFactory MessageFactory { get; }

        /// <summary>
        /// 
        /// </summary>
        ITypeConverter TypeConverter { get; }

        /// <summary>
        /// Функция получения доски опционов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode);
    }
}

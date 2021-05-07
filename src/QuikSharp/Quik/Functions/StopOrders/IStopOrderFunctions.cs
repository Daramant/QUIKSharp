using QuikSharp.DataStructures;
using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.StopOrders
{
    public interface IStopOrderFunctions
    {
        /// <summary>
        /// Возвращает список всех стоп-заявок.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync();

        /// <summary>
        /// Возвращает список стоп-заявок для заданного инструмента.
        /// </summary>
        Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync(string classCode, string securityCode);

        Task<long> CreateStopOrderAsync(StopOrder stopOrder);

        Task<long> KillStopOrderAsync(StopOrder stopOrder);
    }
}

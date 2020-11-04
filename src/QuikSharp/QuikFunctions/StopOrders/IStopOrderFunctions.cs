using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuickFunctions.StopOrders
{
    public interface IStopOrderFunctions
    {
        /// <summary>
        /// Возвращает список всех стоп-заявок.
        /// </summary>
        /// <returns></returns>
        Task<List<StopOrder>> GetStopOrders();

        /// <summary>
        /// Возвращает список стоп-заявок для заданного инструмента.
        /// </summary>
        Task<List<StopOrder>> GetStopOrders(string classCode, string securityCode);

        Task<long> CreateStopOrder(StopOrder stopOrder);

        Task<long> KillStopOrder(StopOrder stopOrder);
    }
}

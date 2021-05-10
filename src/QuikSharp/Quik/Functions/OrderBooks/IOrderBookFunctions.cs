using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.OrderBooks
{
    /// <summary>
    /// Функции для заказа стакана котировок.
    /// </summary>
    public interface IOrderBookFunctions
    {
        /// <summary>
        /// Функция заказывает на сервер получение стакана по указанному классу и инструменту. 
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<bool> SubscribeAsync(string classCode, string secCode);

        /// <summary>
        /// Функция отменяет заказ на получение с сервера стакана по указанному классу и инструменту. 
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<bool> UnsubscribeAsync(string classCode, string secCode);

        /// <summary>
        /// Функция позволяет узнать, заказан ли с сервера стакан по указанному классу и инструменту. 
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <returns></returns>
        Task<bool> IsSubscribedAsync(string classCode, string secCode);
    }
}

using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.OrderBooks
{
    /// <summary>
    /// Функции для работы со стаканом котировок
    /// </summary>
    public interface IOrderBookFunctions
    {
        /// <summary>
        /// Функция заказывает на сервер получение стакана по указанному классу и бумаге.
        /// </summary>
        Task<bool> SubscribeAsync(string class_code, string sec_code);

        /// <summary>
        /// Функция заказывает на сервер получение стакана
        /// </summary>
        Task<bool> SubscribeAsync(ISecurity security);

        /// <summary>
        /// Функция отменяет заказ на получение с сервера стакана по указанному классу и бумаге.
        /// </summary>
        Task<bool> UnsubscribeAsync(string class_code, string sec_code);

        /// <summary>
        /// Функция отменяет заказ на получение с сервера стакана
        /// </summary>
        Task<bool> UnsubscribeAsync(ISecurity security);

        /// <summary>
        /// Функция позволяет узнать, заказан ли с сервера стакан по указанному классу и бумаге.
        /// </summary>
        Task<bool> IsSubscribedAsync(string class_code, string sec_code);

        /// <summary>
        /// Функция позволяет узнать, заказан ли с сервера стакан
        /// </summary>
        Task<bool> IsSubscribedAsync(ISecurity security);

        /// <summary>
        /// Функция предназначена для получения стакана по указанному классу и инструменту
        /// </summary>
        Task<OrderBook> GetQuoteLevel2Async(string class_code, string sec_code);
    }
}

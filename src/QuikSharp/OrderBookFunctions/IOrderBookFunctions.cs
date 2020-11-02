using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.OrderBookFunctions
{
    /// <summary>
    /// Функции для работы со стаканом котировок
    /// </summary>
    public interface IOrderBookFunctions : IQuikService
    {
        /// <summary>
        /// Функция заказывает на сервер получение стакана по указанному классу и бумаге.
        /// </summary>
        Task<bool> Subscribe(string class_code, string sec_code);

        /// <summary>
        /// Функция заказывает на сервер получение стакана
        /// </summary>
        Task<bool> Subscribe(ISecurity security);

        /// <summary>
        /// Функция отменяет заказ на получение с сервера стакана по указанному классу и бумаге.
        /// </summary>
        Task<bool> Unsubscribe(string class_code, string sec_code);

        /// <summary>
        /// Функция отменяет заказ на получение с сервера стакана
        /// </summary>
        Task<bool> Unsubscribe(ISecurity security);

        /// <summary>
        /// Функция позволяет узнать, заказан ли с сервера стакан по указанному классу и бумаге.
        /// </summary>
        Task<bool> IsSubscribed(string class_code, string sec_code);

        /// <summary>
        /// Функция позволяет узнать, заказан ли с сервера стакан
        /// </summary>
        Task<bool> IsSubscribed(ISecurity security);

        /// <summary>
        /// Функция предназначена для получения стакана по указанному классу и инструменту
        /// </summary>
        Task<OrderBook> GetQuoteLevel2(string class_code, string sec_code);
    }
}

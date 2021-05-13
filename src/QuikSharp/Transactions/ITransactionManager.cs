using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Transactions
{
    /// <summary>
    /// Менеджер для работы с транзакциями.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Создание новой заявки.
        /// </summary>
        /// <param name="order">Инфомация о новой заявки, на основе которой будет сформирована транзакция.</param>
        Task<long> SendOrderAsync(Order order);

        /// <summary>
        /// Создание "лимитрированной"заявки.
        /// </summary>
        /// <param name="classCode">Код класса инструмента</param>
        /// <param name="securityCode">Код инструмента</param>
        /// <param name="accountID">Счет клиента</param>
        /// <param name="operation">Операция заявки (покупка/продажа)</param>
        /// <param name="price">Цена заявки</param>
        /// <param name="qty">Количество (в лотах)</param>
        Task<Order> SendLimitOrderAsync(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty);

        /// <summary>
        /// Создание "рыночной"заявки.
        /// </summary>
        /// <param name="classCode">Код класса инструмента</param>
        /// <param name="securityCode">Код инструмента</param>
        /// <param name="accountID">Счет клиента</param>
        /// <param name="operation">Операция заявки (покупка/продажа)</param>
        /// <param name="qty">Количество (в лотах)</param>
        Task<Order> SendMarketOrderAsync(string classCode, string securityCode, string accountID, Operation operation, int qty);

        /// <summary>
        /// Отмена заявки.
        /// </summary>
        /// <param name="order">Информация по заявке, которую требуется отменить.</param>
        Task<long> KillOrderAsync(Order order);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopOrder"></param>
        /// <returns></returns>
        Task<long> CreateStopOrderAsync(StopOrder stopOrder);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopOrder"></param>
        /// <returns></returns>
        Task<long> KillStopOrderAsync(StopOrder stopOrder);
    }
}

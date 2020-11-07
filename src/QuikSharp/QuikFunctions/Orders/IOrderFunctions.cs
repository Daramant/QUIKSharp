using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Orders
{
    public interface IOrderFunctions
    {
        /// <summary>
        /// Создание новой заявки.
        /// </summary>
        /// <param name="order">Инфомация о новой заявки, на основе которой будет сформирована транзакция.</param>
        Task<long> CreateOrderAsync(Order order);

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
        /// Возвращает заявку из хранилища терминала по её номеру.
        /// На основе: http://help.qlua.org/ch4_5_1_1.htm
        /// </summary>
        /// <param name="classCode">Класс инструмента.</param>
        /// <param name="orderId">Номер заявки.</param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(string classCode, long orderId);

        /// <summary>
        /// Возвращает список всех заявок.
        /// </summary>
        /// <returns></returns>
        Task<List<Order>> GetOrdersAsync();

        /// <summary>
        /// Возвращает список заявок для заданного инструмента.
        /// </summary>
        Task<List<Order>> GetOrdersAsync(string classCode, string securityCode);

        /// <summary>
        /// Возвращает заявку для заданного инструмента по ID.
        /// </summary>
        Task<Order> GetOrderByTransactionIdAsync(string classCode, string securityCode, long trans_id);

        /// <summary>
        /// Возвращает заявку по номеру.
        /// </summary>
        Task<Order> GetOrderByNumberAsync(long order_num);
    }
}

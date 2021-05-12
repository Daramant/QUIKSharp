using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.TableRows
{
    public class TableRowFunctions : FunctionsBase, ITableRowFunctions
    {
        public TableRowFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<string> GetClientCodeAsync()
        {
            return ExecuteCommandAsync<string, string>("getClientCode", string.Empty);
        }

        /// <inheritdoc/>
        public Task<string> GetTradeAccountAsync(string classCode)
        {
            return ExecuteCommandAsync<string, string>("getTradeAccount", classCode);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<TradesAccounts>> GetTradeAccountsAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<TradesAccounts>>("getTradeAccounts", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<MoneyLimitEx>> GetMoneyLimitsAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<MoneyLimitEx>>("getMoneyLimits", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<FuturesLimits>> GetFuturesClientLimitsAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<FuturesLimits>>("getFuturesClientLimits", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Order>> GetOrdersAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<Order>>("getOrders", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Order>> GetOrdersAsync(string classCode, string securityCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Order>>("getOrders", new[] { classCode, securityCode });
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderByTransactionIdAsync(string classCode, string securityCode, long transactionId)
        {
            return ExecuteCommandAsync<Order>("getOrderById", new[] { classCode, securityCode, TypeConverter.ToString(transactionId) });
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderByOrderNumberAsync(long orderNumber)
        {
            return ExecuteCommandAsync<string, Order>("getOrderByNumber", TypeConverter.ToString(orderNumber));
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<DepoLimitEx>>("getDepoLimits", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync(string secCode)
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<DepoLimitEx>>("getDepoLimits", secCode);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<Trade>>("getTrades", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Trade>>("getTrades", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesByOrderNumberAsync(long orderNumber)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Trade>>("getTradesByOrderNumber", new[] { TypeConverter.ToString(orderNumber) });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<StopOrder>>("getStopOrders", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<StopOrder>> GetStopOrdersAsync(string classCode, string securityCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<StopOrder>>("getStopOrders", new[] { classCode, securityCode });
        }

        /// <inheritdoc/>
        public Task<Order> GetOrderByNumberAsync(string classCode, long orderId)
        {
            return ExecuteCommandAsync<Order>("getOrderByNumber", new[] { classCode, TypeConverter.ToString(orderId) });
        }
    }
}

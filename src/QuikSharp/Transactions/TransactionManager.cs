using Microsoft.Extensions.Logging;
using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Providers;
using QuikSharp.Quik;
using QuikSharp.Transactions.PersistentStorages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Transactions
{
    /// <summary>
    /// Менеджер для работы с транзакциями.
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        private readonly IQuik _quik;
        private readonly IIdProvider _idProvider;
        private readonly IPersistentStorage _persistentStorage;
        private readonly ILogger<TransactionManager> _logger;

        public TransactionManager(
            IQuik quik,
            IIdProvider idProvider,
            IPersistentStorage persistentStorage,
            ILogger<TransactionManager> logger)
        {
            _quik = quik;
            _idProvider = idProvider;
            _persistentStorage = persistentStorage;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<long> SendOrderAsync(Order order)
        {
            var newOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_ORDER,
                ACCOUNT = order.Account,
                CLASSCODE = order.ClassCode,
                SECCODE = order.SecCode,
                QUANTITY = order.Quantity,
                OPERATION = order.Operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S,
                PRICE = order.Price,
                CLIENT_CODE = order.ClientCode
            };
            return SendTransactionAsync(newOrderTransaction);
        }

        /// <inheritdoc/>
        public Task<Order> SendLimitOrderAsync(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty)
        {
            return SendOrder(classCode, securityCode, accountID, operation, price, qty, TransactionType.L);
        }

        /// <inheritdoc/>
        public Task<Order> SendMarketOrderAsync(string classCode, string securityCode, string accountID, Operation operation, int qty)
        {
            return SendOrder(classCode, securityCode, accountID, operation, 0, qty, TransactionType.M);
        }

        /// <inheritdoc/>
        private async Task<Order> SendOrder(string classCode, string securityCode, string accountID, Operation operation, decimal price, int qty, TransactionType orderType)
        {
            long res = 0;
            bool set = false;
            
            var orderResult = new Order();

            var newOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_ORDER,
                ACCOUNT = accountID,
                CLASSCODE = classCode,
                SECCODE = securityCode,
                QUANTITY = qty,
                OPERATION = operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S,
                PRICE = price,
                TYPE = orderType
            };
            try
            {
                res = await SendTransactionAsync(newOrderTransaction).ConfigureAwait(false);
                //Thread.Sleep(500);
                Console.WriteLine("res: " + res);
            }
            catch
            {
                //ignore
            }

            while (!set)
            {
                if (res > 0)
                {
                    try
                    {
                        orderResult = await _quik.Functions.TableRows.GetOrderByTransactionIdAsync(classCode, securityCode, res).ConfigureAwait(false);
                    }
                    catch
                    {
                        orderResult = new Order { RejectReason = "Неудачная попытка получения заявки по ID-транзакции №" + res };
                    }
                }
                else
                {
                    if (orderResult != null)
                    {
                        orderResult.RejectReason = newOrderTransaction.ErrorMessage;
                    }
                    else
                    {
                        orderResult = new Order { RejectReason = newOrderTransaction.ErrorMessage };
                    }
                }

                if (orderResult != null && (orderResult.RejectReason != "" || orderResult.OrderNum > 0)) set = true;
            }

            return orderResult;
        }

        /// <inheritdoc/>
        public Task<long> KillOrderAsync(Order order)
        {
            var killOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_ORDER,
                CLASSCODE = order.ClassCode,
                SECCODE = order.SecCode,
                ORDER_KEY = _quik.Functions.Custom.TypeConverter.ToString(order.OrderNum)
            };
            return SendTransactionAsync(killOrderTransaction);
        }

        /// <inheritdoc/>
        public Task<long> CreateStopOrderAsync(StopOrder stopOrder)
        {
            var newStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.NEW_STOP_ORDER,
                ACCOUNT = stopOrder.Account,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                EXPIRY_DATE = "GTC", //до отмены
                STOPPRICE = stopOrder.ConditionPrice,
                PRICE = stopOrder.Price,
                QUANTITY = stopOrder.Quantity,
                STOP_ORDER_KIND = ConvertStopOrderType(stopOrder.StopOrderType),
                OPERATION = stopOrder.Operation == Operation.Buy ? TransactionOperation.B : TransactionOperation.S
            };
            if (stopOrder.StopOrderType == StopOrderType.TakeProfit || stopOrder.StopOrderType == StopOrderType.TakeProfitStopLimit)
            {
                newStopOrderTransaction.OFFSET = stopOrder.Offset;
                newStopOrderTransaction.SPREAD = stopOrder.Spread;
                newStopOrderTransaction.OFFSET_UNITS = stopOrder.OffsetUnit;
                newStopOrderTransaction.SPREAD_UNITS = stopOrder.SpreadUnit;
            }

            if (stopOrder.StopOrderType == StopOrderType.TakeProfitStopLimit)
            {
                newStopOrderTransaction.STOPPRICE2 = stopOrder.ConditionPrice2;
            }

            //todo: Not implemented
            //["OFFSET"]=tostring(SysFunc.toPrice(SecCode,MaxOffset)),
            //["OFFSET_UNITS"]="PRICE_UNITS",
            //["SPREAD"]=tostring(SysFunc.toPrice(SecCode,DefSpread)),
            //["SPREAD_UNITS"]="PRICE_UNITS",
            //["MARKET_STOP_LIMIT"]="YES",
            //["MARKET_TAKE_PROFIT"]="YES",
            //["STOPPRICE2"]=tostring(SysFunc.toPrice(SecCode,StopLoss)),
            //["EXECUTION_CONDITION"] = "FILL_OR_KILL",

            return SendTransactionAsync(newStopOrderTransaction);
        }

        private StopOrderKind ConvertStopOrderType(StopOrderType stopOrderType)
        {
            switch (stopOrderType)
            {
                case StopOrderType.StopLimit:
                    return StopOrderKind.SIMPLE_STOP_ORDER;

                case StopOrderType.TakeProfit:
                    return StopOrderKind.TAKE_PROFIT_STOP_ORDER;

                case StopOrderType.TakeProfitStopLimit:
                    return StopOrderKind.TAKE_PROFIT_AND_STOP_LIMIT_ORDER;

                default:
                    throw new Exception("Not implemented stop order type: " + stopOrderType);
            }
        }

        /// <inheritdoc/>
        public Task<long> KillStopOrderAsync(StopOrder stopOrder)
        {
            var killStopOrderTransaction = new Transaction
            {
                ACTION = TransactionAction.KILL_STOP_ORDER,
                CLASSCODE = stopOrder.ClassCode,
                SECCODE = stopOrder.SecCode,
                STOP_ORDER_KEY = _quik.Functions.Custom.TypeConverter.ToString(stopOrder.OrderNum)
            };

            return SendTransactionAsync(killStopOrderTransaction);
        }

        /// <inheritdoc/>
        public async Task<long> SendTransactionAsync(Transaction transaction)
        {
            //Trace.Assert(!transaction.TRANS_ID.HasValue, "TRANS_ID should be assigned automatically in SendTransaction functions");

            //transaction.TRANS_ID = _idProvider.GetNewUniqueId();
            transaction.TRANS_ID = _idProvider.GetNextId();

            //    Console.WriteLine("Trans Id from function = {0}", transaction.TRANS_ID);

            //Trace.Assert(transaction.CLIENT_CODE == null,
            //    "Currently we use Comment to store correlation id for a transaction, " +
            //    "its reply, trades and orders. Support for comments will be added later if needed");
            //// TODO Comments are useful to kill all orders with a single KILL_ALL_ORDERS
            //// But see e.g. this http://www.quik.ru/forum/import/27073/27076/

            //transaction.CLIENT_CODE = transaction.TRANS_ID.Value.ToString();

            if (transaction.CLIENT_CODE == null) 
                transaction.CLIENT_CODE = _quik.Functions.Custom.TypeConverter.ToString(transaction.TRANS_ID.Value);

            //this can be longer than 20 chars.
            //transaction.CLIENT_CODE = _quikClient.PrependWithSessionId(transaction.TRANS_ID.Value);

            try
            {
                var result = await _quik.Functions.Workstation.SendTransactionAsync(transaction);
                //Trace.Assert(result);

                // store transaction
                _persistentStorage.Set(transaction.CLIENT_CODE, transaction);

                return transaction.TRANS_ID.Value;
            }
            catch (TransactionException e)
            {
                transaction.ErrorMessage = e.Message;
                // dirty hack: if transaction was sent we return its id,
                // else we return negative id so the caller will know that
                // the transaction was not sent
                return (-transaction.TRANS_ID.Value);
            }
        }
    }
}

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QuikSharp.Transactions
{
    public class TransactionEventInvoker : ITransactionEventInvoker
    {
        private readonly IPersistentStorage _persistentStorage;
        private readonly ITypeConverter _typeConverter;

        public TransactionEventInvoker(
            IPersistentStorage persistentStorage,
            ITypeConverter typeConverter)
        {
            _persistentStorage = persistentStorage;
            _typeConverter = typeConverter;
        }

        public void OnOrder(Order order)
        {
            // invoke event specific for the transaction
            var correlationId = _typeConverter.ToString(order.TransID);

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + order.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnOrder(order);
                }
            }

            Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all order messages are recieved");
        }

        public void OnStopOrder(StopOrder stopOrder)
        {
            // invoke event specific for the transaction
            string correlationId = _typeConverter.ToString(stopOrder.TransId);

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + stopOrder.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnStopOrder(stopOrder);
                }
            }

            Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all order messages are recieved");
        }

        public void OnTrade(Trade trade)
        {
            // invoke event specific for the transaction
            string correlationId = trade.Comment;

            // ignore unknown transactions
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            #region Totally untested code or handling manual transactions

            if (!_persistentStorage.Contains(correlationId))
            {
                correlationId = "manual:" + trade.OrderNum + ":" + correlationId;
                var fakeTrans = new Transaction()
                {
                    Comment = correlationId,
                    IsManual = true
                    // TODO map order properties back to transaction
                    // ideally, make C# property names consistent (Lua names are set as JSON.NET properties via an attribute)
                };
                _persistentStorage.Set<Transaction>(correlationId, fakeTrans);
            }

            #endregion Totally untested code or handling manual transactions

            var tr = _persistentStorage.Get<Transaction>(correlationId);
            if (tr != null)
            {
                lock (tr)
                {
                    tr.OnTrade(trade);
                    // persist transaction with added trade
                    _persistentStorage.Set(correlationId, tr);
                }
            }

            // ignore unknown transactions
            //Trace.Assert(tr != null, "Transaction must exist in persistent storage until it is completed and all trades messages are recieved");
        }

        public void OnTransReply(TransactionReply reply)
        {
            // invoke event specific for the transaction
            if (string.IsNullOrEmpty(reply.Comment)) //"Initialization user successful" transaction doesn't contain comment
                return;

            if (_persistentStorage.Contains(reply.Comment))
            {
                var tr = _persistentStorage.Get<Transaction>(reply.Comment);
                lock (tr)
                {
                    tr.OnTransReply(reply);
                }
            }
            else
            {
                // NB ignore unmatched transactions
                //Trace.Fail("Transaction must exist in persistent storage until it is completed and its reply is recieved");
            }
        }
    }
}

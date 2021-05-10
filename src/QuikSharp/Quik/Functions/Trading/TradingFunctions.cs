// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Extensions;
using QuikSharp.Providers;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Trading
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
    /// </summary>
    public class TradingFunctions : FunctionsBase, ITradingFunctions
    {
        private readonly IPersistentStorage _persistentStorage;
        private readonly IIdProvider _idProvider;

        public TradingFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter,
            IPersistentStorage persistentStorage,
            IIdProvider idProvider)
            : base(messageFactory, quikClient, typeConverter)
        {
            _persistentStorage = persistentStorage;
            _idProvider = idProvider;
        }

        

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<DepoLimitEx>>("get_depo_limits", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<DepoLimitEx>> GetDepoLimitsAsync(string secCode)
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<DepoLimitEx>>("get_depo_limits", secCode);
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
        public Task<IReadOnlyCollection<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<OptionBoard>>("getOptionBoard", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesAsync()
        {
            return ExecuteCommandAsync<string, IReadOnlyCollection<Trade>>("get_trades", string.Empty);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Trade>>("get_trades", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Trade>> GetTradesByOrderNumberAsync(long orderNum)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Trade>>("get_Trades_by_OrderNumber", new[] { TypeConverter.ToString(orderNum) });
        }

        

        

        /// <inheritdoc/>
        public async Task<long> SendTransactionAsync(Transaction transaction)
        {
            Trace.Assert(!transaction.TRANS_ID.HasValue, "TRANS_ID should be assigned automatically in SendTransaction functions");

            //transaction.TRANS_ID = _idProvider.GetNewUniqueId();
            transaction.TRANS_ID = _idProvider.GetUniqueTransactionId();

            //    Console.WriteLine("Trans Id from function = {0}", transaction.TRANS_ID);

            //Trace.Assert(transaction.CLIENT_CODE == null,
            //    "Currently we use Comment to store correlation id for a transaction, " +
            //    "its reply, trades and orders. Support for comments will be added later if needed");
            //// TODO Comments are useful to kill all orders with a single KILL_ALL_ORDERS
            //// But see e.g. this http://www.quik.ru/forum/import/27073/27076/

            //transaction.CLIENT_CODE = transaction.TRANS_ID.Value.ToString();

            if (transaction.CLIENT_CODE == null) transaction.CLIENT_CODE = TypeConverter.ToString(transaction.TRANS_ID.Value);

            //this can be longer than 20 chars.
            //transaction.CLIENT_CODE = _quikClient.PrependWithSessionId(transaction.TRANS_ID.Value);

            try
            {
                var result = await ExecuteCommandAsync<Transaction, bool>("sendTransaction", transaction);
                Trace.Assert(result);

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
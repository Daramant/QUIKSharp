// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Extensions;
using QuikSharp.IdProviders;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Trading
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
    /// </summary>
    public class TradingFunctions : ITradingFunctions
    {
        private readonly IQuikClient _quikClient;
        private readonly IPersistentStorage _persistentStorage;
        private readonly ITypeConverter _typeConverter;
        private readonly IIdProvider _idProvider;

        public TradingFunctions(
            IQuikClient quikClient,
            IPersistentStorage persistentStorage,
            ITypeConverter typeConverter,
            IIdProvider idProvider)
        {
            _quikClient = quikClient;
            _persistentStorage = persistentStorage;
            _typeConverter = typeConverter;
            _idProvider = idProvider;
        }

        public async Task<DepoLimit> GetDepoAsync(string clientCode, string firmId, string secCode, string account)
        {
            var response = await _quikClient.SendAsync<Result<DepoLimit>>(
                (new Command<string[]>(new[] { clientCode, firmId, secCode, account }, "getDepo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<DepoLimitEx> GetDepoExAsync(string firmId, string clientCode, string secCode, string accID, int limitKind)
        {
            var response = await _quikClient.SendAsync<Result<DepoLimitEx>>(
                (new Command<string[]>(new[] { firmId, clientCode, secCode, accID, _typeConverter.ToStringLookup(limitKind) }, "getDepoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список всех записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimitsAsync()
        {
            var message = new Command<string>(string.Empty, "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikClient.SendAsync<Result<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimitsAsync(string secCode)
        {
            var message = new Command<string>(secCode, "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikClient.SendAsync<Result<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам.
        /// </summary>
        public async Task<MoneyLimit> GetMoneyAsync(string clientCode, string firmId, string tag, string currCode)
        {
            var response = await _quikClient.SendAsync<Result<MoneyLimit>>(
                (new Command<string[]>(new[] { clientCode, firmId, tag, currCode }, "getMoney"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам указанного типа.
        /// </summary>
        public async Task<MoneyLimitEx> GetMoneyExAsync(string firmId, string clientCode, string tag, string currCode, int limitKind)
        {
            var response = await _quikClient.SendAsync<Result<MoneyLimitEx>>(
                (new Command<string[]>(new[] { firmId, clientCode, tag, currCode, _typeConverter.ToStringLookup(limitKind) }, "getMoneyEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют.
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        public async Task<List<MoneyLimitEx>> GetMoneyLimitsAsync()
        {
            var response = await _quikClient.SendAsync<Result<List<MoneyLimitEx>>>(
                (new Command<string>(string.Empty, "getMoneyLimits"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция заказывает получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<bool> ParamRequestAsync(string classCode, string secCode, string paramName)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string[]>(new[] { classCode, secCode, paramName }, "paramRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> ParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string[]>(new[] { classCode, secCode, _typeConverter.ToString(paramName) }, "paramRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция отменяет заказ на получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<bool> CancelParamRequestAsync(string classCode, string secCode, string paramName)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string[]>(new[] { classCode, secCode, paramName }, "cancelParamRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> CancelParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string[]>(new[] { classCode, secCode, _typeConverter.ToString(paramName) }, "cancelParamRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<ParamTable> GetParamExAsync(string classCode, string secCode, string paramName, TimeSpan? timeout = null)
        {
            var response = await _quikClient.SendAsync<Result<ParamTable>>(
                (new Command<string[]>(new[] { classCode, secCode, paramName }, "getParamEx")), timeout).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamExAsync(string classCode, string secCode, ParamName paramName, TimeSpan? timeout = null)
        {
            var response = await _quikClient.SendAsync<Result<ParamTable>>(
                (new Command<string[]>(new[] { classCode, secCode, _typeConverter.ToString(paramName) }, "getParamEx")), timeout).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения всех значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<ParamTable> GetParamEx2Async(string classCode, string secCode, string paramName)
        {
            var response = await _quikClient.SendAsync<Result<ParamTable>>(
                (new Command<string[]>(new[] { classCode, secCode, paramName }, "getParamEx2"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamEx2Async(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikClient.SendAsync<Result<ParamTable>>(
                (new Command<string[]>(new[] { classCode, secCode, _typeConverter.ToString(paramName) }, "getParamEx2"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по фьючерсным лимитам
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="accId"></param>
        /// <param name="limitType"></param>
        /// <param name="currCode"></param>
        /// <returns></returns>
        public async Task<FuturesLimits> GetFuturesLimitAsync(string firmId, string accId, int limitType, string currCode)
        {
            var response = await _quikClient.SendAsync<Result<FuturesLimits>>(
                (new Command<string[]>(new[] { firmId, accId, _typeConverter.ToStringLookup(limitType), currCode }, "getFuturesLimit"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        public async Task<List<FuturesLimits>> GetFuturesClientLimitsAsync()
        {
            var response = await _quikClient.SendAsync<Result<List<FuturesLimits>>>(
                (new Command<string>(string.Empty, "getFuturesClientLimits"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по фьючерсным позициям
        /// </summary>
        /// <param name="firmId"></param>
        /// <param name="accId"></param>
        /// <param name="secCode"></param>
        /// <param name="posType"></param>
        /// <returns></returns>
        public async Task<FuturesClientHolding> GetFuturesHoldingAsync(string firmId, string accId, string secCode, int posType)
        {
            var response = await _quikClient.SendAsync<Result<FuturesClientHolding>>(
                (new Command<string[]>(new[] { firmId, accId, secCode, _typeConverter.ToStringLookup(posType) }, "getFuturesHolding"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode)
        {
            var message = new Command<string[]>(new[] { classCode, secCode }, "getOptionBoard");
            var response = await _quikClient.SendAsync<Result<List<OptionBoard>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesAsync()
        {
            var response = await _quikClient.SendAsync<Result<List<Trade>>>(
                (new Command<string>(string.Empty, "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesAsync(string classCode, string secCode)
        {
            var response = await _quikClient.SendAsync<Result<List<Trade>>>(
                (new Command<string[]>(new[] { classCode, secCode }, "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesByOrderNumberAsync(long orderNum)
        {
            var response = await _quikClient.SendAsync<Result<List<Trade>>>(
                (new Command<string[]>(new[] { _typeConverter.ToString(orderNum) }, "get_Trades_by_OrderNumber"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfo> GetPortfolioInfoAsync(string firmId, string clientCode)
        {
            var response = await _quikClient.SendAsync<Result<PortfolioInfo>>(
                (new Command<string[]>(new[] { firmId, clientCode }, "getPortfolioInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfoEx> GetPortfolioInfoExAsync(string firmId, string clientCode, int limitKind)
        {
            var response = await _quikClient.SendAsync<Result<PortfolioInfoEx>>(
                (new Command<string[]>(new[] { firmId, clientCode, _typeConverter.ToStringLookup(limitKind) }, "getPortfolioInfoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTrdAccByClientCodeAsync(string firmId, string clientCode)
        {
            var response = await _quikClient.SendAsync<Result<string>>(
                (new Command<string[]>(new[] { firmId, clientCode }, "GetTrdAccByClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCodeByTrdAccAsync(string firmId, string trdAccId)
        {
            var response = await _quikClient.SendAsync<Result<string>>(
                (new Command<string[]>(new[] { firmId, trdAccId }, "GetClientCodeByTrdAcc"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsUcpClientAsync(string firmId, string client)
        {
            var response = await _quikClient.SendAsync<Result<bool>>(
                (new Command<string[]>(new[] { firmId, client }, "IsUcpClient"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Send a single transaction to Quik server
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
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

            if (transaction.CLIENT_CODE == null) transaction.CLIENT_CODE = _typeConverter.ToString(transaction.TRANS_ID.Value);

            //this can be longer than 20 chars.
            //transaction.CLIENT_CODE = _quikClient.PrependWithSessionId(transaction.TRANS_ID.Value);

            try
            {
                var response = await _quikClient.SendAsync<Result<bool>>(
                    (new Command<Transaction>(transaction, "sendTransaction"))).ConfigureAwait(false);
                Trace.Assert(response.Data);

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
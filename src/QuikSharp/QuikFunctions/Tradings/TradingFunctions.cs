// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Exceptions;
using QuikSharp.Messages;
using QuikSharp.QuikService;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Tradings
{
    /// <summary>
    /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
    /// </summary>
    public class TradingFunctions : ITradingFunctions
    {
        private readonly IQuikService _quikService;
        private readonly IPersistentStorage _persistentStorage;

        public TradingFunctions(
            IQuikService quikService,
            IPersistentStorage persistentStorage)
        {
            _quikService = quikService;
            _persistentStorage = persistentStorage;
        }

        //public async Task<string[]> GetClassesList() {
        //    var response = await _quikService.Send<Response<string>>(
        //        (new Request<string>("", "getClassesList")));
        //    return response.Data == null
        //        ? new string[0]
        //        : response.Data.TrimEnd(',').Split(new[] { "," }, StringSplitOptions.None);
        //}

        //public async Task<ClassInfo> GetClassInfo(string classID) {
        //    var response = await _quikService.Send<Response<ClassInfo>>(
        //        (new Request<string>(classID, "getClassInfo")));
        //    return response.Data;
        //}

        //public async Task<SecurityInfo> GetSecurityInfo(string classCode, string secCode) {
        //    var response = await _quikService.Send<Response<SecurityInfo>>(
        //        (new Request<string>(classCode + "|" + secCode, "getSecurityInfo")));
        //    return response.Data;
        //}

        //public async Task<SecurityInfo> GetSecurityInfo(ISecurity security) {
        //    return await GetSecurityInfo(security.ClassCode, security.SecCode);
        //}

        //public async Task<string[]> GetClassSecurities(string classID) {
        //    var response = await _quikService.Send<Response<string>>(
        //        (new Request<string>(classID, "getClassSecurities")));
        //    return response.Data == null
        //        ? new string[0]
        //        : response.Data.TrimEnd(',').Split(new[] { "," }, StringSplitOptions.None);
        //}
        public async Task<DepoLimit> GetDepoAsync(string clientCode, string firmId, string secCode, string account)
        {
            var response = await _quikService.SendAsync<Result<DepoLimit>>(
                (new Command<string>(clientCode + "|" + firmId + "|" + secCode + "|" + account, "getDepo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<DepoLimitEx> GetDepoExAsync(string firmId, string clientCode, string secCode, string accID, int limitKind)
        {
            var response = await _quikService.SendAsync<Result<DepoLimitEx>>(
                (new Command<string>(firmId + "|" + clientCode + "|" + secCode + "|" + accID + "|" + limitKind, "getDepoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список всех записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimitsAsync()
        {
            var message = new Command<string>("", "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikService.SendAsync<Result<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimitsAsync(string secCode)
        {
            var message = new Command<string>(secCode, "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikService.SendAsync<Result<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам.
        /// </summary>
        public async Task<MoneyLimit> GetMoneyAsync(string clientCode, string firmId, string tag, string currCode)
        {
            var response = await _quikService.SendAsync<Result<MoneyLimit>>(
                (new Command<string>(clientCode + "|" + firmId + "|" + tag + "|" + currCode, "getMoney"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам указанного типа.
        /// </summary>
        public async Task<MoneyLimitEx> GetMoneyExAsync(string firmId, string clientCode, string tag, string currCode, int limitKind)
        {
            var response = await _quikService.SendAsync<Result<MoneyLimitEx>>(
                (new Command<string>(firmId + "|" + clientCode + "|" + tag + "|" + currCode + "|" + limitKind, "getMoneyEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют.
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        public async Task<List<MoneyLimitEx>> GetMoneyLimitsAsync()
        {
            var response = await _quikService.SendAsync<Result<List<MoneyLimitEx>>>(
                (new Command<string>("", "getMoneyLimits"))).ConfigureAwait(false);
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
            var response = await _quikService.SendAsync<Result<bool>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "paramRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> ParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikService.SendAsync<Result<bool>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "paramRequest"))).ConfigureAwait(false);
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
            var response = await _quikService.SendAsync<Result<bool>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "cancelParamRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> CancelParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikService.SendAsync<Result<bool>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "cancelParamRequest"))).ConfigureAwait(false);
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
        public async Task<ParamTable> GetParamExAsync(string classCode, string secCode, string paramName, int timeout = Timeout.Infinite)
        {
            var response = await _quikService.SendAsync<Result<ParamTable>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx")), timeout).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamExAsync(string classCode, string secCode, ParamName paramName, int timeout = Timeout.Infinite)
        {
            var response = await _quikService.SendAsync<Result<ParamTable>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx")), timeout).ConfigureAwait(false);
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
            var response = await _quikService.SendAsync<Result<ParamTable>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx2"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamEx2Async(string classCode, string secCode, ParamName paramName)
        {
            var response = await _quikService.SendAsync<Result<ParamTable>>(
                (new Command<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx2"))).ConfigureAwait(false);
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
            var response = await _quikService.SendAsync<Result<FuturesLimits>>(
                (new Command<string>(firmId + "|" + accId + "|" + limitType + "|" + currCode, "getFuturesLimit"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        public async Task<List<FuturesLimits>> GetFuturesClientLimitsAsync()
        {
            var response = await _quikService.SendAsync<Result<List<FuturesLimits>>>(
                (new Command<string>("", "getFuturesClientLimits"))).ConfigureAwait(false);
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
            var response = await _quikService.SendAsync<Result<FuturesClientHolding>>(
                (new Command<string>(firmId + "|" + accId + "|" + secCode + "|" + posType, "getFuturesHolding"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode)
        {
            var message = new Command<string>(classCode + "|" + secCode, "getOptionBoard");
            Message<List<OptionBoard>> response =
                await _quikService.SendAsync<Result<List<OptionBoard>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesAsync()
        {
            var response = await _quikService.SendAsync<Result<List<Trade>>>(
                (new Command<string>("", "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesAsync(string classCode, string secCode)
        {
            var response = await _quikService.SendAsync<Result<List<Trade>>>(
                (new Command<string>(classCode + "|" + secCode, "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTradesByOrderNumberAsync(long orderNum)
        {
            var response = await _quikService.SendAsync<Result<List<Trade>>>(
                (new Command<string>(orderNum.ToString(), "get_Trades_by_OrderNumber"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfo> GetPortfolioInfoAsync(string firmId, string clientCode)
        {
            var response = await _quikService.SendAsync<Result<PortfolioInfo>>(
                (new Command<string>(firmId + "|" + clientCode, "getPortfolioInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfoEx> GetPortfolioInfoExAsync(string firmId, string clientCode, int limitKind)
        {
            var response = await _quikService.SendAsync<Result<PortfolioInfoEx>>(
                (new Command<string>(firmId + "|" + clientCode + "|" + limitKind, "getPortfolioInfoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTrdAccByClientCodeAsync(string firmId, string clientCode)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(firmId + "|" + clientCode, "GetTrdAccByClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCodeByTrdAccAsync(string firmId, string trdAccId)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(firmId + "|" + trdAccId, "GetClientCodeByTrdAcc"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsUcpClientAsync(string firmId, string client)
        {
            var response = await _quikService.SendAsync<Result<bool>>(
                (new Command<string>(firmId + "|" + client, "IsUcpClient"))).ConfigureAwait(false);
            return response.Data;
        }

        /*public async Task<ClassInfo> GetClassInfo(string classID) {
            var response = await _quikService.Send<Response<ClassInfo>>(
                (new Request<string>(classID, "getClassInfo")));
            return response.Data;
        }*/

        /// <summary>
        /// Send a single transaction to Quik server
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<long> SendTransactionAsync(Transaction transaction)
        {
            Trace.Assert(!transaction.TRANS_ID.HasValue, "TRANS_ID should be assigned automatically in SendTransaction functions");

            //transaction.TRANS_ID = _quikService.GetNewUniqueId();
            transaction.TRANS_ID = _quikService.GetUniqueTransactionId();

            //    Console.WriteLine("Trans Id from function = {0}", transaction.TRANS_ID);

            //Trace.Assert(transaction.CLIENT_CODE == null,
            //    "Currently we use Comment to store correlation id for a transaction, " +
            //    "its reply, trades and orders. Support for comments will be added later if needed");
            //// TODO Comments are useful to kill all orders with a single KILL_ALL_ORDERS
            //// But see e.g. this http://www.quik.ru/forum/import/27073/27076/

            //transaction.CLIENT_CODE = transaction.TRANS_ID.Value.ToString();

            if (transaction.CLIENT_CODE == null) transaction.CLIENT_CODE = transaction.TRANS_ID.Value.ToString();

            //this can be longer than 20 chars.
            //transaction.CLIENT_CODE = _quikService.PrependWithSessionId(transaction.TRANS_ID.Value);

            try
            {
                var response = await _quikService.SendAsync<Result<bool>>(
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
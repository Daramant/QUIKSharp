// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Messages;
using QuikSharp.QuickService;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuickFunctions.Tradings
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
        //    var response = await _quikService.Send<Message<string>>(
        //        (new Message<string>("", "getClassesList")));
        //    return response.Data == null
        //        ? new string[0]
        //        : response.Data.TrimEnd(',').Split(new[] { "," }, StringSplitOptions.None);
        //}

        //public async Task<ClassInfo> GetClassInfo(string classID) {
        //    var response = await _quikService.Send<Message<ClassInfo>>(
        //        (new Message<string>(classID, "getClassInfo")));
        //    return response.Data;
        //}

        //public async Task<SecurityInfo> GetSecurityInfo(string classCode, string secCode) {
        //    var response = await _quikService.Send<Message<SecurityInfo>>(
        //        (new Message<string>(classCode + "|" + secCode, "getSecurityInfo")));
        //    return response.Data;
        //}

        //public async Task<SecurityInfo> GetSecurityInfo(ISecurity security) {
        //    return await GetSecurityInfo(security.ClassCode, security.SecCode);
        //}

        //public async Task<string[]> GetClassSecurities(string classID) {
        //    var response = await _quikService.Send<Message<string>>(
        //        (new Message<string>(classID, "getClassSecurities")));
        //    return response.Data == null
        //        ? new string[0]
        //        : response.Data.TrimEnd(',').Split(new[] { "," }, StringSplitOptions.None);
        //}
        public async Task<DepoLimit> GetDepo(string clientCode, string firmId, string secCode, string account)
        {
            var response = await _quikService.Send<Message<DepoLimit>>(
                (new Message<string>(clientCode + "|" + firmId + "|" + secCode + "|" + account, "getDepo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<DepoLimitEx> GetDepoEx(string firmId, string clientCode, string secCode, string accID, int limitKind)
        {
            var response = await _quikService.Send<Message<DepoLimitEx>>(
                (new Message<string>(firmId + "|" + clientCode + "|" + secCode + "|" + accID + "|" + limitKind, "getDepoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список всех записей из таблицы 'Лимиты по бумагам'.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimits()
        {
            var message = new Message<string>("", "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikService.Send<Message<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Возвращает список записей из таблицы 'Лимиты по бумагам', отфильтрованных по коду инструмента.
        /// </summary>
        public async Task<List<DepoLimitEx>> GetDepoLimits(string secCode)
        {
            var message = new Message<string>(secCode, "get_depo_limits");
            Message<List<DepoLimitEx>> response = await _quikService.Send<Message<List<DepoLimitEx>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам.
        /// </summary>
        public async Task<MoneyLimit> GetMoney(string clientCode, string firmId, string tag, string currCode)
        {
            var response = await _quikService.Send<Message<MoneyLimit>>(
                (new Message<string>(clientCode + "|" + firmId + "|" + tag + "|" + currCode, "getMoney"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения информации по денежным лимитам указанного типа.
        /// </summary>
        public async Task<MoneyLimitEx> GetMoneyEx(string firmId, string clientCode, string tag, string currCode, int limitKind)
        {
            var response = await _quikService.Send<Message<MoneyLimitEx>>(
                (new Message<string>(firmId + "|" + clientCode + "|" + tag + "|" + currCode + "|" + limitKind, "getMoneyEx"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по денежным лимитам всех торговых счетов (кроме фьючерсных) и валют.
        ///  Лучшее место для получения связки clientCode + firmid
        /// </summary>
        public async Task<List<MoneyLimitEx>> GetMoneyLimits()
        {
            var response = await _quikService.Send<Message<List<MoneyLimitEx>>>(
                (new Message<string>("", "getMoneyLimits"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция заказывает получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<bool> ParamRequest(string classCode, string secCode, string paramName)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "paramRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> ParamRequest(string classCode, string secCode, ParamNames paramName)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "paramRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция отменяет заказ на получение параметров Таблицы текущих торгов
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<bool> CancelParamRequest(string classCode, string secCode, string paramName)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "cancelParamRequest"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> CancelParamRequest(string classCode, string secCode, ParamNames paramName)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "cancelParamRequest"))).ConfigureAwait(false);
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
        public async Task<ParamTable> GetParamEx(string classCode, string secCode, string paramName, int timeout = Timeout.Infinite)
        {
            var response = await _quikService.Send<Message<ParamTable>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx")), timeout).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamEx(string classCode, string secCode, ParamNames paramName, int timeout = Timeout.Infinite)
        {
            var response = await _quikService.Send<Message<ParamTable>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx")), timeout).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Функция для получения всех значений Таблицы текущих значений параметров
        /// </summary>
        /// <param name="classCode"></param>
        /// <param name="secCode"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public async Task<ParamTable> GetParamEx2(string classCode, string secCode, string paramName)
        {
            var response = await _quikService.Send<Message<ParamTable>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx2"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<ParamTable> GetParamEx2(string classCode, string secCode, ParamNames paramName)
        {
            var response = await _quikService.Send<Message<ParamTable>>(
                (new Message<string>(classCode + "|" + secCode + "|" + paramName, "getParamEx2"))).ConfigureAwait(false);
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
        public async Task<FuturesLimits> GetFuturesLimit(string firmId, string accId, int limitType, string currCode)
        {
            var response = await _quikService.Send<Message<FuturesLimits>>(
                (new Message<string>(firmId + "|" + accId + "|" + limitType + "|" + currCode, "getFuturesLimit"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        ///  функция для получения информации по фьючерсным лимитам всех клиентских счетов
        /// </summary>
        public async Task<List<FuturesLimits>> GetFuturesClientLimits()
        {
            var response = await _quikService.Send<Message<List<FuturesLimits>>>(
                (new Message<string>("", "getFuturesClientLimits"))).ConfigureAwait(false);
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
        public async Task<FuturesClientHolding> GetFuturesHolding(string firmId, string accId, string secCode, int posType)
        {
            var response = await _quikService.Send<Message<FuturesClientHolding>>(
                (new Message<string>(firmId + "|" + accId + "|" + secCode + "|" + posType, "getFuturesHolding"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<OptionBoard>> GetOptionBoard(string classCode, string secCode)
        {
            var message = new Message<string>(classCode + "|" + secCode, "getOptionBoard");
            Message<List<OptionBoard>> response =
                await _quikService.Send<Message<List<OptionBoard>>>(message).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTrades()
        {
            var response = await _quikService.Send<Message<List<Trade>>>(
                (new Message<string>("", "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTrades(string classCode, string secCode)
        {
            var response = await _quikService.Send<Message<List<Trade>>>(
                (new Message<string>(classCode + "|" + secCode, "get_trades"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<Trade>> GetTrades_by_OdrerNumber(long orderNum)
        {
            var response = await _quikService.Send<Message<List<Trade>>>(
                (new Message<string>(orderNum.ToString(), "get_Trades_by_OrderNumber"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfo> GetPortfolioInfo(string firmId, string clientCode)
        {
            var response = await _quikService.Send<Message<PortfolioInfo>>(
                (new Message<string>(firmId + "|" + clientCode, "getPortfolioInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<PortfolioInfoEx> GetPortfolioInfoEx(string firmId, string clientCode, int limitKind)
        {
            var response = await _quikService.Send<Message<PortfolioInfoEx>>(
                (new Message<string>(firmId + "|" + clientCode + "|" + limitKind, "getPortfolioInfoEx"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTrdAccByClientCode(string firmId, string clientCode)
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>(firmId + "|" + clientCode, "GetTrdAccByClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCodeByTrdAcc(string firmId, string trdAccId)
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>(firmId + "|" + trdAccId, "GetClientCodeByTrdAcc"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsUcpClient(string firmId, string client)
        {
            var response = await _quikService.Send<Message<bool>>(
                (new Message<string>(firmId + "|" + client, "IsUcpClient"))).ConfigureAwait(false);
            return response.Data;
        }

        /*public async Task<ClassInfo> GetClassInfo(string classID) {
            var response = await _quikService.Send<Message<ClassInfo>>(
                (new Message<string>(classID, "getClassInfo")));
            return response.Data;
        }*/

        /// <summary>
        /// Send a single transaction to Quik server
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<long> SendTransaction(Transaction transaction)
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
                var response = await _quikService.Send<Message<bool>>(
                    (new Message<Transaction>(transaction, "sendTransaction"))).ConfigureAwait(false);
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
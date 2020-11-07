// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Classes
{
    /// <summary>
    /// Функции для обращения к спискам доступных параметров
    /// </summary>
    public class ClassFunctions : IClassFunctions
    {
        private readonly IQuikService _quikService;

        public ClassFunctions(IQuikService quikService)
        {
            _quikService = quikService;
        }

        public async Task<string[]> GetClassesListAsync()
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>("", "getClassesList"))).ConfigureAwait(false);
            return response.Data == null
                ? new string[0]
                : response.Data.TrimEnd(',').Split(new[] {","}, StringSplitOptions.None);
        }

        public async Task<ClassInfo> GetClassInfoAsync(string classID)
        {
            var response = await _quikService.SendAsync<Result<ClassInfo>>(
                (new Command<string>(classID, "getClassInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<SecurityInfo> GetSecurityInfoAsync(string classCode, string secCode)
        {
            var response = await _quikService.SendAsync<Result<SecurityInfo>>(
                (new Command<string>(classCode + "|" + secCode, "getSecurityInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<SecurityInfo> GetSecurityInfoAsync(ISecurity security)
        {
            return await GetSecurityInfoAsync(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<string[]> GetClassSecuritiesAsync(string classID)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(classID, "getClassSecurities"))).ConfigureAwait(false);
            return response.Data == null
                ? new string[0]
                : response.Data.TrimEnd(',').Split(new[] {","}, StringSplitOptions.None);
        }

        public async Task<string> GetSecurityClassAsync(string classesList, string secCode)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(classesList + "|" + secCode, "getSecurityClass"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCodeAsync()
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>("", "getClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTradeAccountAsync(string classCode)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(classCode, "getTradeAccount"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<TradesAccounts>> GetTradeAccountsAsync()
        {
            var response = await _quikService.SendAsync<Result<List<TradesAccounts>>>(
                (new Command<string>("", "getTradeAccounts"))).ConfigureAwait(false);
            return response.Data;
        }
    }
}
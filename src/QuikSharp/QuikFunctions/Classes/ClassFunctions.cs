// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
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
        private readonly IQuikClient _quikClient;

        public ClassFunctions(IQuikClient quikClient)
        {
            _quikClient = quikClient;
        }

        public async Task<string[]> GetClassesListAsync()
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(string.Empty, "getClassesList"))).ConfigureAwait(false);

            return response.Data == null
                ? Array.Empty<string>()
                : response.Data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<ClassInfo> GetClassInfoAsync(string classID)
        {
            var response = await _quikClient.SendAsync<IResult<ClassInfo>>(
                (new Command<string>(classID, "getClassInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<SecurityInfo> GetSecurityInfoAsync(string classCode, string secCode)
        {
            var response = await _quikClient.SendAsync<IResult<SecurityInfo>>(
                (new Command<string[]>(new[] { classCode, secCode }, "getSecurityInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public Task<SecurityInfo> GetSecurityInfoAsync(ISecurity security)
        {
            return GetSecurityInfoAsync(security.ClassCode, security.SecCode);
        }

        public async Task<string[]> GetClassSecuritiesAsync(string classID)
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(classID, "getClassSecurities"))).ConfigureAwait(false);
            return response.Data == null
                ? Array.Empty<string>()
                : response.Data.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<string> GetSecurityClassAsync(string classesList, string secCode)
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string[]>(new[] { classesList, secCode }, "getSecurityClass"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCodeAsync()
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(string.Empty, "getClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTradeAccountAsync(string classCode)
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(classCode, "getTradeAccount"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<TradesAccounts>> GetTradeAccountsAsync()
        {
            var response = await _quikClient.SendAsync<IResult<List<TradesAccounts>>>(
                (new Command<string>(string.Empty, "getTradeAccounts"))).ConfigureAwait(false);
            return response.Data;
        }
    }
}
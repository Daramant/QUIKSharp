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

        public async Task<string[]> GetClassesList()
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>("", "getClassesList"))).ConfigureAwait(false);
            return response.Data == null
                ? new string[0]
                : response.Data.TrimEnd(',').Split(new[] {","}, StringSplitOptions.None);
        }

        public async Task<ClassInfo> GetClassInfo(string classID)
        {
            var response = await _quikService.Send<Response<ClassInfo>>(
                (new Request<string>(classID, "getClassInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<SecurityInfo> GetSecurityInfo(string classCode, string secCode)
        {
            var response = await _quikService.Send<Response<SecurityInfo>>(
                (new Request<string>(classCode + "|" + secCode, "getSecurityInfo"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<SecurityInfo> GetSecurityInfo(ISecurity security)
        {
            return await GetSecurityInfo(security.ClassCode, security.SecCode).ConfigureAwait(false);
        }

        public async Task<string[]> GetClassSecurities(string classID)
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>(classID, "getClassSecurities"))).ConfigureAwait(false);
            return response.Data == null
                ? new string[0]
                : response.Data.TrimEnd(',').Split(new[] {","}, StringSplitOptions.None);
        }

        public async Task<string> GetSecurityClass(string classesList, string secCode)
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>(classesList + "|" + secCode, "getSecurityClass"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetClientCode()
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>("", "getClientCode"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetTradeAccount(string classCode)
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>(classCode, "getTradeAccount"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<List<TradesAccounts>> GetTradeAccounts()
        {
            var response = await _quikService.Send<Response<List<TradesAccounts>>>(
                (new Request<string>("", "getTradeAccounts"))).ConfigureAwait(false);
            return response.Data;
        }
    }
}
// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Classes
{
    /// <summary>
    /// Функции для обращения к спискам доступных параметров
    /// </summary>
    public class ClassFunctions : FunctionsBase, IClassFunctions
    {
        private static readonly char[] Separators = new[] { ',' };

        public ClassFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public async Task<string[]> GetClassesListAsync()
        {
            var classes = await ExecuteCommandAsync<string, string>("getClassesList", string.Empty);
            return classes?.Split(Separators, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }

        /// <inheritdoc/>
        public Task<ClassInfo> GetClassInfoAsync(string classCode)
        {
            return ExecuteCommandAsync<string, ClassInfo>("getClassInfo", classCode);
        }

        /// <inheritdoc/>
        public async Task<string[]> GetClassSecuritiesAsync(string classID)
        {
            var securities = await ExecuteCommandAsync<string, string>("getClassSecurities", classID);
            return securities?.Split(Separators, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }



        


        /// <inheritdoc/>
        public Task<string> GetSecurityClassAsync(string classesList, string secCode)
        {
            return ExecuteCommandAsync<string>("getSecurityClass", new[] { classesList, secCode });
        }

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
    }
}
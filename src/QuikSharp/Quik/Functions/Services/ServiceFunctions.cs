// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.TypeConverters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Services
{
    /// <summary>
    /// Сервисные функции.
    /// </summary>
    public class ServiceFunctions : FunctionsBase, IServiceFunctions
    {
        public ServiceFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<bool> IsConnectedAsync(TimeSpan? timeout = null)
        {
            return ExecuteCommandAsync<string, bool>("isConnected", string.Empty);
        }

        /// <inheritdoc/>
        public Task<string> GetScriptPathAsync()
        {
            return ExecuteCommandAsync<string, string>("getScriptPath", string.Empty);
        }

        /// <inheritdoc/>
        public Task<string> GetInfoParamAsync(InfoParamName paramName)
        {
            return ExecuteCommandAsync<string, string>("getInfoParam", TypeConverter.ToString(paramName));
        }

        /// <inheritdoc/>
        public Task<bool> MessageAsync(string message, IconType iconType = IconType.Info)
        {
            return ExecuteCommandAsync<bool>("message", new[] { message, TypeConverter.ToStringLookup((int)iconType) });
        }

        /// <inheritdoc/>
        public Task<string> GetWorkingFolderAsync()
        {
            return ExecuteCommandAsync<string, string>("getWorkingFolder", string.Empty);
        }

        /// <inheritdoc/>
        public Task PrintDbgStrAsync(string message)
        {
            return ExecuteCommandAsync<string, string>("PrintDbgStr", message);
        }

        /// <inheritdoc/>
        public Task<QuikDateTime> SysDateAsync()
        {
            return ExecuteCommandAsync<string, QuikDateTime>("sysdate", string.Empty);
        }
    }
}
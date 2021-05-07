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
    /// Service functions implementations
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
        public Task<string> GetWorkingFolderAsync()
        {
            return ExecuteCommandAsync<string, string>("getWorkingFolder", string.Empty);
        }

        /// <inheritdoc/>
        public async Task<bool> IsConnectedAsync(TimeSpan? timeout = null)
        {
            return await ExecuteCommandAsync<string, string>("isConnected", string.Empty) == "1";
        }

        /// <inheritdoc/>
        public Task<string> GetScriptPathAsync()
        {
            return ExecuteCommandAsync<string, string>("getScriptPath", string.Empty);
        }

        /// <inheritdoc/>
        public Task<string> GetInfoParamAsync(InfoParams param)
        {
            return ExecuteCommandAsync<string, string>("getInfoParam", TypeConverter.ToString(param));
        }

        /// <inheritdoc/>
        public Task MessageAsync(string message, NotificationType iconType = NotificationType.Info)
        {
            return ExecuteCommandAsync<string>("message", new[] { message, TypeConverter.ToStringLookup((int)iconType) });
        }

        /// <inheritdoc/>
        public Task PrintDbgStrAsync(string message)
        {
            return ExecuteCommandAsync<string, string>("PrintDbgStr", message);
        }
    }
}
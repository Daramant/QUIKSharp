// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
using QuikSharp.QuikFunctions.Services;
using QuikSharp.TypeConverters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Services
{
    /// <summary>
    /// Service functions implementations
    /// </summary>
    public class ServiceFunctions : IServiceFunctions
    {
        private readonly IQuikClient _quikClient;
        private readonly ITypeConverter _typeConverter;

        public ServiceFunctions(
            IQuikClient quikClient,
            ITypeConverter typeConverter)
        {
            _quikClient = quikClient;
            _typeConverter = typeConverter;
        }

        public async Task<string> GetWorkingFolderAsync()
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(string.Empty, "getWorkingFolder"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsConnectedAsync(TimeSpan? timeout = null)
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(string.Empty, "isConnected")), timeout).ConfigureAwait(false);
            return response.Data == "1";
        }

        public async Task<string> GetScriptPathAsync()
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(string.Empty, "getScriptPath"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetInfoParamAsync(InfoParams param)
        {
            var response = await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(_typeConverter.ToString(param), "getInfoParam"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> MessageAsync(string message, NotificationType iconType = NotificationType.Info)
        {
            switch (iconType)
            {
                case NotificationType.Info:
                    await _quikClient.SendAsync<IResult<string>>(
                        (new Command<string>(message, "message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Warning:
                    await _quikClient.SendAsync<IResult<string>>(
                        (new Command<string>(message, "warning_message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Error:
                    await _quikClient.SendAsync<IResult<string>>(
                        (new Command<string>(message, "error_message"))).ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("iconType");
            }

            return true; // TODO: Возвращать результат из Quik.
        }

        public async Task<bool> PrintDbgStrAsync(string message)
        {
            await _quikClient.SendAsync<IResult<string>>(
                (new Command<string>(message, "PrintDbgStr"))).ConfigureAwait(false);
            return true; // TODO: Возвращать результат из Quik.
        }
    }
}
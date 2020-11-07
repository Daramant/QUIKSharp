// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.QuikService;
using QuikSharp.QuikFunctions.Services;
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
        private readonly IQuikService _quikService;

        public ServiceFunctions(IQuikService quikService)
        {
            _quikService = quikService;
        }

        public async Task<string> GetWorkingFolderAsync()
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>("", "getWorkingFolder"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsConnectedAsync(int timeout = Timeout.Infinite)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>("", "isConnected")), timeout).ConfigureAwait(false);
            return response.Data == "1";
        }

        public async Task<string> GetScriptPathAsync()
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>("", "getScriptPath"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetInfoParamAsync(InfoParams param)
        {
            var response = await _quikService.SendAsync<Result<string>>(
                (new Command<string>(param.ToString(), "getInfoParam"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> MessageAsync(string message, NotificationType iconType = NotificationType.Info)
        {
            switch (iconType)
            {
                case NotificationType.Info:
                    await _quikService.SendAsync<Result<string>>(
                        (new Command<string>(message, "message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Warning:
                    await _quikService.SendAsync<Result<string>>(
                        (new Command<string>(message, "warning_message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Error:
                    await _quikService.SendAsync<Result<string>>(
                        (new Command<string>(message, "error_message"))).ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("iconType");
            }

            return true;
        }

        public async Task<bool> PrintDbgStrAsync(string message)
        {
            await _quikService.SendAsync<Result<string>>(
                (new Command<string>(message, "PrintDbgStr"))).ConfigureAwait(false);
            return true;
        }

        public async Task<double> AddLabelAsync(double price, string curDate, string curTime, string hint, string path, string tag, string alignment, double backgnd)
        {
            var response = await _quikService.SendAsync<Result<double>>(
                    (new Command<string>(price + "|" + curDate + "|" + curTime + "|" + hint + "|" + path + "|" + tag + "|" + alignment + "|" + backgnd, "addLabel")))
                .ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> DelLabelAsync(string tag, double id)
        {
            await _quikService.SendAsync<Result<string>>(
                (new Command<string>(tag + "|" + id, "delLabel"))).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DelAllLabelsAsync(string tag)
        {
            await _quikService.SendAsync<Result<string>>(
                (new Command<string>(tag, "delAllLabels"))).ConfigureAwait(false);
            return true;
        }
    }
}
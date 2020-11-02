// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.QuickService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.ServiceFunctions
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

        public async Task<string> GetWorkingFolder()
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>("", "getWorkingFolder"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> IsConnected(int timeout = Timeout.Infinite)
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>("", "isConnected")), timeout).ConfigureAwait(false);
            return response.Data == "1";
        }

        public async Task<string> GetScriptPath()
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>("", "getScriptPath"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<string> GetInfoParam(InfoParams param)
        {
            var response = await _quikService.Send<Message<string>>(
                (new Message<string>(param.ToString(), "getInfoParam"))).ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> Message(string message, NotificationType iconType = NotificationType.Info)
        {
            switch (iconType)
            {
                case NotificationType.Info:
                    await _quikService.Send<Message<string>>(
                        (new Message<string>(message, "message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Warning:
                    await _quikService.Send<Message<string>>(
                        (new Message<string>(message, "warning_message"))).ConfigureAwait(false);
                    break;

                case NotificationType.Error:
                    await _quikService.Send<Message<string>>(
                        (new Message<string>(message, "error_message"))).ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("iconType");
            }

            return true;
        }

        public async Task<bool> PrintDbgStr(string message)
        {
            await _quikService.Send<Message<string>>(
                (new Message<string>(message, "PrintDbgStr"))).ConfigureAwait(false);
            return true;
        }

        public async Task<double> AddLabel(double price, string curDate, string curTime, string hint, string path, string tag, string alignment, double backgnd)
        {
            var response = await _quikService.Send<Message<double>>(
                    (new Message<string>(price + "|" + curDate + "|" + curTime + "|" + hint + "|" + path + "|" + tag + "|" + alignment + "|" + backgnd, "addLabel")))
                .ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> DelLabel(string tag, double id)
        {
            await _quikService.Send<Message<string>>(
                (new Message<string>(tag + "|" + id, "delLabel"))).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DelAllLabels(string tag)
        {
            await _quikService.Send<Message<string>>(
                (new Message<string>(tag, "delAllLabels"))).ConfigureAwait(false);
            return true;
        }

        public void InitializeCorrelationId(int startCorrelationId)
        {
            _quikService.InitializeCorrelationId(startCorrelationId);
        }
    }
}
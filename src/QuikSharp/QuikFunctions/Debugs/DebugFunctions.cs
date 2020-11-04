// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.QuickService;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuikSharp.QuickFunctions.Debugs
{
    public class DebugFunctions : IDebugFunctions
    {
        private readonly IQuikService _quikService;

        public DebugFunctions(IQuikService quikService)
        {
            _quikService = quikService;
        }

        private class PingRequest : Request<string>
        {
            public PingRequest()
                : base("Ping", "ping", null)
            {
            }
        }

        private class PingResponse : Response<string>
        {
            public PingResponse()
                : base("Pong")
            {
            }
        }

        public async Task<string> Ping()
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            var response = await _quikService.Send<PingResponse>((new PingRequest())).ConfigureAwait(false);
            Trace.Assert(response.Data == "Pong");
            return response.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> Echo<T>(T msg)
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            var response = await _quikService.Send<Response<T>>(
                (new Request<T>(msg, "echo"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// This method returns LuaException and demonstrates how Lua errors are caught
        /// </summary>
        /// <returns></returns>
        public async Task<string> DivideStringByZero()
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>("", "divide_string_by_zero"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Check if running inside Quik
        /// </summary>
        public async Task<bool> IsQuik()
        {
            var response = await _quikService.Send<Response<string>>(
                (new Request<string>("", "is_quik"))).ConfigureAwait(false);
            return response.Data == "1";
        }
    }
}
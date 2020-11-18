// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.QuikClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Debug
{
    public class DebugFunctions : IDebugFunctions
    {
        private readonly IQuikClient _quikClient;

        public DebugFunctions(IQuikClient quikClient)
        {
            _quikClient = quikClient;
        }

        private class PingRequest : Command<string>
        {
            public PingRequest()
                : base("Ping", "ping", null)
            {
            }
        }

        private class PingResponse : Result<string>
        {
            public PingResponse()
                : base("Pong")
            {
            }
        }

        public async Task<string> PingAsync()
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            var response = await _quikClient.SendAsync<PingResponse>((new PingRequest())).ConfigureAwait(false);
            Trace.Assert(response.Data == "Pong");
            return response.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> EchoAsync<T>(T msg)
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            var response = await _quikClient.SendAsync<Result<T>>(
                (new Command<T>(msg, "echo"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// This method returns LuaException and demonstrates how Lua errors are caught
        /// </summary>
        /// <returns></returns>
        public async Task<string> DivideStringByZeroAsync()
        {
            var response = await _quikClient.SendAsync<Result<string>>(
                (new Command<string>(string.Empty, "divide_string_by_zero"))).ConfigureAwait(false);
            return response.Data;
        }

        /// <summary>
        /// Check if running inside Quik
        /// </summary>
        public async Task<bool> IsQuikAsync()
        {
            var response = await _quikClient.SendAsync<Result<string>>(
                (new Command<string>(string.Empty, "is_quik"))).ConfigureAwait(false);
            return response.Data == "1";
        }
    }
}
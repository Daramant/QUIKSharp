// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Debug
{
    /// <summary>
    /// Функции для отладки работы QuikSharp.
    /// </summary>
    public class DebugFunctions : FunctionsBase, IDebugFunctions
    {
        public DebugFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base (messageFactory, quikClient, typeConverter)
        { }

        private class PingRequest : Command<string>
        {
            public PingRequest()
                : base("Ping", "ping")
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

        /// <inheritdoc/>
        public async Task<string> PingAsync()
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            var command = new PingRequest();
            var result = await QuikClient.SendAsync<PingResponse>(command).ConfigureAwait(false);
            Trace.Assert(result.Data == "Pong");
            return result.Data;
        }

        /// <inheritdoc/>
        public Task<T> EchoAsync<T>(T msg)
        {
            return ExecuteCommandAsync<T, T>("echo", msg);
        }

        /// <inheritdoc/>
        public Task<string> DivideStringByZeroAsync()
        {
            return ExecuteCommandAsync<string, string>("divideStringByZero", string.Empty);
        }

        /// <inheritdoc/>
        public async Task<bool> IsQuikAsync()
        {
            return await ExecuteCommandAsync<string, string>("isQuik", string.Empty) == "1";
        }
    }
}
// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Debug
{
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
            var response = await QuikClient.SendAsync<PingResponse>((new PingRequest())).ConfigureAwait(false);
            Trace.Assert(response.Data == "Pong");
            return response.Data;
        }

        /// <inheritdoc/>
        public Task<T> EchoAsync<T>(T msg)
        {
            // could have used StringMessage directly. This is an example of how to define DTOs for custom commands
            return ExecuteCommandAsync<T, T>("echo", msg);
        }

        /// <inheritdoc/>
        public Task<string> DivideStringByZeroAsync()
        {
            return ExecuteCommandAsync<string, string>("divide_string_by_zero", string.Empty);
        }

        /// <inheritdoc/>
        public async Task<bool> IsQuikAsync()
        {
            return await ExecuteCommandAsync<string, string>("is_quik", string.Empty) == "1";
        }
    }
}
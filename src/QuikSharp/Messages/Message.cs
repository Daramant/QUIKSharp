// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;
using System;

namespace QuikSharp.Messages
{
    /// <summary>
    /// Default generic implementation
    /// </summary>
    internal class Message<T> : BaseMessage
    {
        public Message()
        {
        }

        public Message(T message, string command, DateTime? validUntil = null)
        {
            Command = command;
            CreatedTime = DateTime.Now.Ticks / 10000L - Epoch;
            ValidUntil = validUntil;
            Data = message;
        }

        /// <summary>
        /// String message
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
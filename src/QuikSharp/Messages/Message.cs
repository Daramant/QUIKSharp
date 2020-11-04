// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;
using QuikSharp.Tools;
using System;

namespace QuikSharp.Messages
{
    /// <summary>
    /// Default generic implementation
    /// </summary>
    internal class Message<T> : IMessage<T>
    {
        public Message()
        {
        }

        public Message(T data)
        {
            Data = data;
            CreatedTime = DateTimeTool.GetCurrentTime();
        }

        /// <summary>
        /// Timestamp in milliseconds, same as in Lua `socket.gettime() * 1000`
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public long CreatedTime { get; set; }

        /// <summary>
        /// String message
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    internal abstract class BaseMessage : IMessage
    {
        protected static readonly long Epoch = (new DateTime(1970, 1, 1, 3, 0, 0, 0)).Ticks / 10000L;

        protected BaseMessage(string command = "", DateTime? validUntil = null)
        {
            Command = command;
            CreatedTime = DateTime.Now.Ticks / 10000L - Epoch;
            ValidUntil = validUntil;
        }

        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        [JsonProperty(PropertyName = "cmd")]
        public string Command { get; set; }

        /// <summary>
        /// Timestamp in milliseconds, same as in Lua `socket.gettime() * 1000`
        /// </summary>
        [JsonProperty(PropertyName = "t")]
        public long CreatedTime { get; set; }

        /// <summary>
        /// Some messages are valid only for a short time, e.g. buy/sell orders
        /// </summary>
        [JsonProperty(PropertyName = "v")]
        public DateTime? ValidUntil { get; set; }
    }
}

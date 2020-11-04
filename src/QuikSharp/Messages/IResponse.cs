using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IResponse : IMessage
    {
        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        long Id { get; set; }
    }

    public interface IResponse<T> : IMessage<T>, IResponse
    {
    }
}

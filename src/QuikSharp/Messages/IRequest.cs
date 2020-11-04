using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IRequest: IMessage
    {
        /// <summary>
        /// Unique correlation id to match requests and responses
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Some messages are valid only for a short time, e.g. buy/sell orders
        /// </summary>
        DateTime? ValidUntil { get; set; }
    }

    public interface IRequest<T> : IMessage<T>, IRequest
    {
    }
}

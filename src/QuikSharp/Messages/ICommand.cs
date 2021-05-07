using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface ICommand : IMessage
    {
        /// <summary>
        /// Идентификатор команды.
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

    public interface ICommand<T> : IMessage<T>, ICommand
    {
    }
}

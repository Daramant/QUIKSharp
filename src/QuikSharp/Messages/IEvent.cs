using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IEvent : IMessage
    {
        /// <summary>
        /// A name of a function to call for requests
        /// </summary>
        EventName Name { get; set; }
    }

    public interface IEvent<T> : IMessage<T>, IEvent
    {
    }
}

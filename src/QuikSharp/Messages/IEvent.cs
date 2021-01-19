using QuikSharp.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Messages
{
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Тип события.
        /// </summary>
        EventName Name { get; set; }
    }

    public interface IEvent<T> : IMessage<T>, IEvent
    {
    }
}

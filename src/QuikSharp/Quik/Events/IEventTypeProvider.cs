using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Events
{
    public interface IEventTypeProvider
    {
        bool TryGetEventType(EventName eventName, out Type eventType);
    }
}

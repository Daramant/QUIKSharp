using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuikSharp.Exceptions
{
    [Serializable]
    public class EventTypeNotSupportedException : QuikSharpException
    {
        public EventTypeNotSupportedException()
            : base()
        { }

        public EventTypeNotSupportedException(string message)
            : base(message)
        { }

        public EventTypeNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected EventTypeNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

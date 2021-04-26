using QuikSharp.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuikSharp.Exceptions
{
    [Serializable]
    public class CommandTimeoutException : QuikSharpException
    {
        public CommandTimeoutException()
            : base()
        { }

        public CommandTimeoutException(string message)
            : base(message)
        { }

        public CommandTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected CommandTimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

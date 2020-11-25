using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuikSharp.Exceptions
{
    /// <summary>
    /// An exception caught on Lua side with a message from Lua
    /// </summary>
    [Serializable]
    public class QuikException : Exception
    {
        public QuikException()
            : base()
        { }

        public QuikException(string message)
            : base(message)
        { }

        public QuikException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected QuikException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

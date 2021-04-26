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
    public class QuikSharpException : Exception
    {
        public QuikSharpException()
            : base()
        { }

        public QuikSharpException(string message)
            : base(message)
        { }

        public QuikSharpException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected QuikSharpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

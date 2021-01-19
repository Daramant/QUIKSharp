using QuikSharp.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuikSharp.Serialization.Exceptions
{
    public class QuikSerializationException : QuikException
    {
        public QuikSerializationException()
            : base()
        { }

        public QuikSerializationException(string message)
            : base(message)
        { }

        public QuikSerializationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected QuikSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

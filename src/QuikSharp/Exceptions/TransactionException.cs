using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QuikSharp.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class TransactionException : QuikException
    {
        public TransactionException() 
            : base()
        { }

        public TransactionException(string message) 
            : base(message)
        { }

        public TransactionException(string message, Exception innerException) 
            : base(message, innerException)
        { }

        protected TransactionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

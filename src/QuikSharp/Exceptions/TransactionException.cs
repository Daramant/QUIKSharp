using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Exceptions
{
    /// <summary>
    ///
    /// </summary>
    public class TransactionException : LuaException
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public TransactionException(string message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Exceptions
{
    /// <summary>
    /// An exception caught on Lua side with a message from Lua
    /// </summary>
    public class QuikException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public QuikException(string message)
            : base(message)
        {
        }
    }
}

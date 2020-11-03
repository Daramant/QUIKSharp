using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Exceptions
{
    /// <summary>
    /// An exception caught on Lua side with a message from Lua
    /// </summary>
    public class LuaException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public LuaException(string message)
            : base(message)
        {
        }
    }
}

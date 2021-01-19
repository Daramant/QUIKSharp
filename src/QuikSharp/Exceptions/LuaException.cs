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
    public class LuaException : QuikException
    {
        public LuaException()
            : base()
        { }

        public LuaException(string message)
            : base(message)
        { }

        public LuaException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected LuaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

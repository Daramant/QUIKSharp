using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Timestamp in milliseconds, same as in Lua `socket.gettime() * 1000`.
        /// </summary>
        long NowInMilliseconds { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime UtcNow { get; }
    }
}

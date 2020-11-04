using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Debugs
{
    public interface IDebugFunctions
    {
        Task<string> Ping();

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> Echo<T>(T msg);

        /// <summary>
        /// This method returns LuaException and demonstrates how Lua errors are caught
        /// </summary>
        /// <returns></returns>
        Task<string> DivideStringByZero();

        /// <summary>
        /// Check if running inside Quik
        /// </summary>
        Task<bool> IsQuik();
    }
}

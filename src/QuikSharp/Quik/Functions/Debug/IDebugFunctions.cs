using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Debug
{
    public interface IDebugFunctions
    {
        Task<string> PingAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> EchoAsync<T>(T msg);

        /// <summary>
        /// This method returns LuaException and demonstrates how Lua errors are caught
        /// </summary>
        /// <returns></returns>
        Task<string> DivideStringByZeroAsync();

        /// <summary>
        /// Check if running inside Quik
        /// </summary>
        Task<bool> IsQuikAsync();
    }
}

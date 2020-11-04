using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace QuikSharp.Extensions
{
    public static class SocketExtensions
    {
        /// <summary>
        /// The connection state of a socket is reflected in the Connected property,
        /// but this property is only updated with the last send or receive action.
        /// To determine the connection state before send or receive the one and only way
        /// is polling the state directly from the socket itself. The following
        /// extension class does this.
        /// </summary>
        public static bool IsConnected(this Socket s)
        {
            var part1 = s.Poll(1000, SelectMode.SelectRead);
            var part2 = s.Available == 0;

            return (!part1 || !part2) && s.Connected;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikClient
{
    /// <summary>
    /// A handler for events without arguments
    /// </summary>
    public delegate void VoidHandler();

    /// <summary>
    /// Обработчик события OnInit
    /// </summary>
    /// <param name="port">Порт обмена данными</param>
    public delegate void InitHandler(int port);

    /// <summary>
    /// Обработчик события OnStop
    /// </summary>
    public delegate void StopHandler(int signal);
}

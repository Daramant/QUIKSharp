using QuikSharp.QuickService;
using QuikSharp.QuikEvents;
using QuikSharp.QuikFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuick
    {
        IQuikService Service { get; }

        IQuikFunctions Functions { get; }

        /// <summary>
        /// Функции обратного вызова
        /// </summary>
        IQuikEvents Events { get; }

        /// <summary>
        /// Persistent transaction storage
        /// </summary>
        IPersistentStorage Storage { get; }
    }
}

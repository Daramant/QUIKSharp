using QuikSharp.QuikService;
using QuikSharp.QuikEvents;
using QuikSharp.QuikFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuik
    {
        IQuikService Service { get; }

        IQuikFunctions Functions { get; }

        /// <summary>
        /// Функции обратного вызова
        /// </summary>
        IQuikEvents Events { get; }
    }
}

using QuikSharp.Messages;
using QuikSharp.QuikEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikClient
{
    public interface IQuikClient
    {
        #region Events

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp успешно подключилась к Quik'у
        /// </summary>
        event QuikEventHandler<InitEventArgs> Connected;

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp была отключена от Quik'а
        /// </summary>
        event QuikEventHandler<EventArgs> Disconnected;

        #endregion Events

        void Start();

        Task StopAsync();

        Task<TResult> SendAsync<TResult>(ICommand request, TimeSpan? timeout = null)
            where TResult : class, IResult;
    }
}

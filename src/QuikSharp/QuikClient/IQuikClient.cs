using QuikSharp.Messages;
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
        event InitHandler Connected;

        /// <summary>
        /// Событие вызывается когда библиотека QuikSharp была отключена от Quik'а
        /// </summary>
        event VoidHandler Disconnected;

        /// <summary>
        /// Функция вызывается терминалом QUIK при остановке скрипта из диалога управления.
        /// Примечание: Значение параметра «stop_flag» – «1».После окончания выполнения функции таймаут завершения работы скрипта 5 секунд. По истечении этого интервала функция main() завершается принудительно. При этом возможна потеря системных ресурсов.
        /// </summary>
        event StopHandler Stop;

        #endregion Events

        void Start();

        Task StopAsync();

        Task<TResult> SendAsync<TResult>(ICommand request, int timeout = 0)
            where TResult : class, IResult, new();

        int GetUniqueTransactionId();
    }
}

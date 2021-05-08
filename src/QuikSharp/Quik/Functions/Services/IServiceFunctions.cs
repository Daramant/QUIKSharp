using QuikSharp.DataStructures;
using QuikSharp.Quik.Functions.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Services
{
    /// <summary>
    /// Сервисные функции.
    /// </summary>
    public interface IServiceFunctions
    {
        /// <summary>
        /// Функция предназначена для определения состояния подключения клиентского места к серверу.
        /// </summary>
        /// <returns><c>True</c>, если клиентсвое место подключено, иначе - <c>False</c>.</returns>
        Task<bool> IsConnectedAsync(TimeSpan? timeout = null);

        /// <summary>
        /// Функция возвращает путь, по которому находится запускаемый скрипт, без завершающего обратного слэша («\»). Например, C:\QuikFront\Scripts
        /// </summary>
        /// <returns>Путь, по которому находится запускаемый скрипт.</returns>
        Task<string> GetScriptPathAsync();

        /// <summary>
        /// Функция возвращает значения параметров информационного окна (пункт меню Система / О программе / Информационное окно…). 
        /// </summary>
        /// <param name="paramName">Имя параметра.</param>
        /// <returns>Значение параметра.</returns>
        Task<string> GetInfoParamAsync(InfoParamName paramName);

        /// <summary>
        /// Функция отображает сообщения в терминале QUIK.
        /// </summary>
        /// <param name="message">Строка, отображаемая в окне сообщений терминала QUIK.</param>
        /// <param name="iconType">Тип отображаемой иконки в сообщении.</param>
        /// <returns>Возвращает <c>False</c> при ошибке выполнения или при обнаружении ошибки во входных параметрах. В остальных случаях возвращает <c>True</c>.</returns>
        Task<bool> MessageAsync(string message, IconType iconType);

        /// <summary>
        /// Функция возвращает путь, по которому находится файл info.exe, исполняющий данный скрипт, без завершающего обратного слэша («\»). Например, C:\QuikFront.
        /// </summary>
        /// <returns>Путь, по которому находится файл info.exe.</returns>
        Task<string> GetWorkingFolderAsync();

        /// <summary>
        /// Функция для вывода отладочной информации.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <returns></returns>
        Task PrintDbgStrAsync(string message);

        /// <summary>
        /// Функция возвращает системные дату и время с точностью до микросекунд.
        /// </summary>
        /// <returns>Системные дата и время.</returns>
        Task<QuikDateTime> SysDateAsync();
    }
}

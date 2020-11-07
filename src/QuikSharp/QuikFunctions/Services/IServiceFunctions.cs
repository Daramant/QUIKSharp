﻿using QuikSharp.QuikFunctions.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Services
{
    /// <summary>
    /// Сервисные функции
    /// </summary>
    public interface IServiceFunctions
    {
        /// <summary>
        /// Функция возвращает путь, по которому находится файл info.exe, исполняющий данный скрипт, без завершающего обратного слэша («\»). Например, C:\QuikFront.
        /// </summary>
        /// <returns></returns>
        Task<string> GetWorkingFolderAsync();

        /// <summary>
        /// Функция предназначена для определения состояния подключения клиентского места к серверу. Возвращает «1», если клиентское место подключено и «0», если не подключено.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsConnectedAsync(int timeout = Timeout.Infinite);

        /// <summary>
        /// Функция возвращает путь, по которому находится запускаемый скрипт, без завершающего обратного слэша («\»). Например, C:\QuikFront\Scripts
        /// </summary>
        /// <returns></returns>
        Task<string> GetScriptPathAsync();

        /// <summary>
        /// Функция возвращает значения параметров информационного окна (пункт меню Связь / Информационное окно…).
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<string> GetInfoParamAsync(InfoParams param);

        /// <summary>
        /// Функция отображает сообщения в терминале QUIK.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="iconType"></param>
        /// <returns></returns>
        Task<bool> MessageAsync(string message, NotificationType iconType);

        Task<bool> PrintDbgStrAsync(string message);

        /// <summary>
        /// Выводит метку на график
        /// </summary>
        /// <param name="price"></param>
        /// <param name="curDate"></param>
        /// <param name="curTime"></param>
        /// <param name="hint"></param>
        /// <param name="path"></param>
        /// <param name="tag"></param>
        /// <param name="alignment">LEFT, RIGHT, TOP, BOTTOM</param>
        /// <param name="backgnd"> On = 1, Off = 0</param>
        /// <returns>Возвращает Id метки</returns>
        Task<double> AddLabelAsync(double price, string curDate, string curTime, string hint, string path, string tag, string alignment, double backgnd);

        /// <summary>
        /// Удаляет метку по ее Id
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id">Id метки</param>
        /// <returns></returns>
        Task<bool> DelLabelAsync(string tag, double id);

        /// <summary>
        /// Удаляет все метки с графика
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task<bool> DelAllLabelsAsync(string tag);
    }
}

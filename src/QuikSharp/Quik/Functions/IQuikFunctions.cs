using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.Classes;
using QuikSharp.Quik.Functions.Debug;
using QuikSharp.Quik.Functions.Labels;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.Orders;
using QuikSharp.Quik.Functions.QuotesTableParameters;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.Quik.Functions.StopOrders;
using QuikSharp.Quik.Functions.TableRows;
using QuikSharp.Quik.Functions.Trading;
using QuikSharp.Quik.Functions.Workplace;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Functions
{
    public interface IQuikFunctions
    {
        /// <summary>
        /// Сервисные функции.
        /// </summary>
        IServiceFunctions Service { get; }

        /// <summary>
        /// Функции для обращения к строкам произвольных таблиц QUIK.
        /// </summary>
        ITableRowFunctions TableRows { get; set; }

        /// <summary>
        /// Функции для обращения к спискам доступных параметров.
        /// </summary>
        IClassFunctions Class { get; }

        /// <summary>
        /// Функции взаимодействия скрипта Lua и Рабочего места QUIK.
        /// </summary>
        IWorkstationFunctions Workstation { get; set; }

        /// <summary>
        /// Функции для работы с графиками.
        /// </summary>
        ICandleFunctions Candles { get; }

        /// <summary>
        /// Функции для работы с метками.
        /// </summary>
        ILabelFunctions Labels { get; }

        /// <summary>
        /// Функции для заказа стакана котировок.
        /// </summary>
        IOrderBookFunctions OrderBook { get; }

        /// <summary>
        /// Функции для заказа параметров Таблицы текущих торгов.
        /// </summary>
        IQuotesTableParametersFunctions QuotesTableParameters { get; set; }

        /// <summary>
        /// Функции для отладки работы QuikSharp.
        /// </summary>
        IDebugFunctions Debug { get; }
    }
}

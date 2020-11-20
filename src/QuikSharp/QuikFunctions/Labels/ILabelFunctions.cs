using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Labels
{
    public interface ILabelFunctions
    {
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
        Task<decimal> AddLabelAsync(decimal price, string curDate, string curTime, string hint, string path, string tag, string alignment, decimal backgnd);

        /// <summary>
        /// Удаляет метку по ее Id
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="id">Id метки</param>
        /// <returns></returns>
        Task<bool> DelLabelAsync(string tag, decimal id);

        /// <summary>
        /// Удаляет все метки с графика
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task<bool> DelAllLabelsAsync(string tag);
    }
}

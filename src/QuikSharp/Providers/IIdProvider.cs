using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    /// <summary>
    /// Провайдер идентификаторов.
    /// </summary>
    public interface IIdProvider
    {
        /// <summary>
        /// Устанавливает стартовое значение идентификатора.
        /// </summary>
        /// <param name="startId">Стартовое значение идентификатора.</param>
        void SetStartId(long startId);

        /// <summary>
        /// Возвращает следующий идентификатор.
        /// </summary>
        /// <returns>Идентификатор.</returns>
        long GetNextId();
    }
}

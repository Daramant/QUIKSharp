using QuikSharp.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace QuikSharp.Providers
{
    /// <summary>
    /// Провайдер уникальных идентификаторов.
    /// </summary>
    public class UniqueIdProvider : IIdProvider
    {
        private readonly object _syncRoot = new object();

        private long _lastId;

        /// <inheritdoc/>
        public void SetStartId(long startId)
        {
            if (startId < 0)
                throw new QuikSharpException($"Стартовое значение идентификатора должно быть >= 0. Указано: {startId}.");

            _lastId = startId;
        }

        /// <inheritdoc/>
        public long GetNextId()
        {
            var newId = Interlocked.Increment(ref _lastId);

            if (newId > 0)
                return newId;

            lock (_syncRoot)
            {
                if (_lastId <= 0)
                {
                    _lastId = newId = 1;
                }
                else
                {
                    newId = Interlocked.Increment(ref _lastId);
                }

                return _lastId;
            }
        }
    }
}

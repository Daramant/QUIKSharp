using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace QuikSharp.Providers
{
    public class IdProvider : IIdProvider
    {
        internal const int UniqueIdOffset = 0;
        internal MemoryMappedFile mmf;
        internal MemoryMappedViewAccessor accessor;

        /// <summary>
        /// Current correlation id. Use Interlocked.Increment to get a new id.
        /// </summary>
        private static int _correlationId;

        /// <summary>
        /// info.exe file path
        /// </summary>
        public string WorkingFolder { get; private set; }

        private readonly object _syncRoot = new object();

        /// <summary>
        /// Generate a new unique ID for current session
        /// </summary>
        public int GetUniqueCommandId()
        {
            lock (_syncRoot)
            {
                var newId = Interlocked.Increment(ref _correlationId);
                // 2^31 = 2147483648
                // with 1 000 000 messages per second it will take more than
                // 35 hours to overflow => safe for use as TRANS_ID in SendTransaction
                // very weird stuff: Уникальный идентификационный номер заявки, значение от 1 до 2 294 967 294
                if (newId > 0)
                {
                    return newId;
                }

                _correlationId = 1;
                return 1;
            }
        }

        /// <summary>
        /// Get or Generate unique transaction ID for function SendTransaction()
        /// </summary>
        public int GetUniqueTransactionId()
        {
            if (mmf == null || accessor == null)
            {
                if (String.IsNullOrEmpty(WorkingFolder)) //WorkingFolder = Не определено. Создаем MMF в памяти
                {
                    mmf = MemoryMappedFile.CreateOrOpen("UniqueID", 4096);
                }
                else //WorkingFolder определен. Открываем MMF с диска
                {
                    string diskFileName = WorkingFolder + "\\" + "QUIKSharp.Settings";
                    try
                    {
                        mmf = MemoryMappedFile.CreateFromFile(diskFileName, FileMode.OpenOrCreate, "UniqueID", 4096);
                    }
                    catch
                    {
                        mmf = MemoryMappedFile.CreateOrOpen("UniqueID", 4096);
                    }
                }

                accessor = mmf.CreateViewAccessor();
            }

            int newId = accessor.ReadInt32(UniqueIdOffset);
            if (newId == 0)
            {
                newId = Convert.ToInt32(DateTime.Now.ToString("ddHHmmss"));
            }
            else
            {
                if (newId >= 2147483638)
                {
                    newId = 100;
                }
                newId++;
            }

            try
            {
                accessor.Write(UniqueIdOffset, newId);
            }
            catch (Exception er)
            {
                Console.WriteLine("Неудачная попытка записини нового ID в файл MMF: " + er.Message);
            }

            return newId;
        }

        /// <summary>
        /// Устанавливает стартовое значение для CorrelactionId.
        /// </summary>
        /// <param name="startCorrelationId">Стартовое значение.</param>
        public void InitializeCorrelationId(int startCorrelationId)
        {
            _correlationId = startCorrelationId;
        }
    }
}

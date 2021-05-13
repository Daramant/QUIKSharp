using QuikSharp.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace QuikSharp.Transactions
{
    public class TransactionIdProvider : IIdProvider
    {
        internal const int UniqueIdOffset = 0;
        internal MemoryMappedFile mmf;
        internal MemoryMappedViewAccessor accessor;

        /// <summary>
        /// Current correlation id. Use Interlocked.Increment to get a new id.
        /// </summary>
        private long _lastId;

        /// <summary>
        /// info.exe file path
        /// </summary>
        public string WorkingFolder { get; private set; }

        private readonly object _syncRoot = new object();

        /// <inheritdoc/>
        public long GetNextId()
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

        /// <inheritdoc/>
        public void SetStartId(long startId)
        {
            _lastId = startId;
        }
    }
}

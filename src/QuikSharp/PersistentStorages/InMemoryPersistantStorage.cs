using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.PersistentStorages
{
    /// <summary>
    /// Thread-unsafe
    /// </summary>
    public class InMemoryPersistantStorage : IPersistentStorage
    {
        private static readonly IDictionary<string, object> Dic
            = new Dictionary<string, object>();

        private object syncRoot = new object();

        /// <summary>
        /// Useful for more advanced manipulation than IPersistentStorage
        /// QuikSharp depends only on IPersistentStorage
        /// </summary>
        private static IDictionary<string, object> Storage
        {
            get { return Dic; }
        }

        public void Set<T>(string key, T value)
        {
            lock (syncRoot)
            {
                Dic[key] = value;
            }
        }

        public T Get<T>(string key)
        {
            lock (syncRoot)
            {
                var v = (T)Dic[key];
                return (T)v;
            }
        }

        public bool Contains(string key)
        {
            lock (syncRoot)
            {
                if (Dic.ContainsKey(key))
                {
                    return true;
                }

                return false;
            }
        }

        public bool Remove(string key)
        {
            lock (syncRoot)
            {
                var s = Dic.Remove(key);
                return s;
            }
        }
    }
}

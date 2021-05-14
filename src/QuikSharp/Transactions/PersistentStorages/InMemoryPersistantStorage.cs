using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Transactions.PersistentStorages
{
    /// <summary>
    /// Thread-unsafe
    /// </summary>
    public class InMemoryPersistantStorage : IPersistentStorage
    {
        private static readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public void Set<T>(string key, T value)
        {
            _dictionary[key] = value;
        }

        public T Get<T>(string key)
        {
            return (T)_dictionary[key];
        }

        public bool Contains(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dictionary.TryRemove(key, out _);
        }
    }
}

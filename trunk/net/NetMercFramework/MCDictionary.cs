using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace it.mintlab.desktopnet.mercframework
{
    /// <summary>
    /// Thread safe custom dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class MCDictionary<T> : IDictionary<MessageContent,T>
    {
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private Dictionary<string, List<KeyValuePair<MessageContent,T>>> map;


        public MCDictionary()
        {
            map = new Dictionary<string, List<KeyValuePair<MessageContent,T>>>();
        }


        #region IDictionary<MessageContent,T> Membri di

        public void Add(MessageContent key, T value)
        {
            List<KeyValuePair<MessageContent, T>> list;
            string k = key.getKey();

            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    list = map[k];
                }
                else
                {
                    list = new List<KeyValuePair<MessageContent, T>>();
                    LockCookie lc = rwLock.UpgradeToWriterLock(Timeout.Infinite);
                    try { map.Add(key.getKey(), list); }
                    finally { rwLock.DowngradeFromWriterLock(ref lc); }
                }
                list.Add(new KeyValuePair<MessageContent, T>(key, value));
            }
            finally { rwLock.ReleaseReaderLock(); }
        }

        public bool ContainsKey(MessageContent key)
        {
            string k = key.getKey();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    foreach (KeyValuePair<MessageContent, T> pair in map[k])
                    {
                        if (pair.Key.Equals(key))
                            return true;
                    }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            return false;
        }

        public ICollection<MessageContent> Keys
        {
            get
            {
                ICollection<MessageContent> result = new List<MessageContent>();
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                    {
                        foreach (KeyValuePair<MessageContent, T> pair in list)
                        {
                            result.Add(pair.Key);
                        }
                    }
                }
                finally { rwLock.ReleaseReaderLock(); }
                return result;
            }
        }

        public bool Remove(MessageContent key)
        {
            string k = key.getKey();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    foreach (KeyValuePair<MessageContent, T> pair in map[k])
                    {
                        if (pair.Key.Equals(key))
                        {
                            LockCookie lc = rwLock.UpgradeToWriterLock(Timeout.Infinite);
                            bool result = false;
                            try
                            {
                                result = map[k].Remove(pair);
                                if (map[k].Count == 0) map.Remove(k);
                            }
                            finally { rwLock.DowngradeFromWriterLock(ref lc); }
                            return result;
                        }
                    }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            return false;
        }

        public bool TryGetValue(MessageContent key, out T value)
        {
            value = default(T);
            string k = key.getKey();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    foreach (KeyValuePair<MessageContent, T> pair in map[k])
                    {
                        if (pair.Key.Equals(key))
                        {
                            value = pair.Value;
                            return true;
                        }
                    }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            return false;
        }

        public ICollection<T> Values
        {
            get
            {
                ICollection<T> result = new List<T>();
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                    {
                        foreach (KeyValuePair<MessageContent, T> pair in list)
                        {
                            result.Add(pair.Value);
                        }
                    }
                }
                finally { rwLock.ReleaseReaderLock(); }
                return result;
            }
        }

        public T this[MessageContent key]
        {
            get
            {
                string k = key.getKey();
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    if (map.ContainsKey(k))
                    {
                        foreach (KeyValuePair<MessageContent, T> pair in map[k])
                        {
                            if (pair.Key.Equals(key))
                            {
                                return pair.Value;
                            }
                        }
                    }
                }
                finally { rwLock.ReleaseReaderLock(); }
                throw new ArgumentException("No key found");
            }
            set
            {
                List<KeyValuePair<MessageContent,T>> list = new List<KeyValuePair<MessageContent,T>>();
                rwLock.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    map.Add(key.getKey(), list);
                }
                finally { rwLock.ReleaseWriterLock(); }
                list.Add(new KeyValuePair<MessageContent,T>(key, value));
            }
        }

        #endregion

        #region ICollection<KeyValuePair<MessageContent,T>> Membri di

        public void Add(KeyValuePair<MessageContent, T> item)
        {
            List<KeyValuePair<MessageContent, T>> list;
            string k = item.Key.getKey();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    list = map[k];
                }
                else
                {
                    list = new List<KeyValuePair<MessageContent, T>>();
                    LockCookie lc = rwLock.UpgradeToWriterLock(Timeout.Infinite);
                    try { map.Add(item.Key.getKey(), list); }
                    finally { rwLock.DowngradeFromWriterLock(ref lc); }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            list.Add(new KeyValuePair<MessageContent, T>(item.Key, item.Value));
        }

        public void Clear()
        {
            rwLock.AcquireWriterLock(Timeout.Infinite);
            try { map.Clear(); }
            finally { rwLock.ReleaseWriterLock(); }
        }

        public bool Contains(KeyValuePair<MessageContent, T> item)
        {
            return ((IDictionary)this)[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<MessageContent, T>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get
            {
                int result = 0;
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                    {
                        result += list.Count;
                    }
                }
                finally { rwLock.ReleaseReaderLock(); }
                return result;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<MessageContent, T> item)
        {
            string k = item.Key.getKey();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (map.ContainsKey(k))
                {
                    foreach (KeyValuePair<MessageContent, T> pair in map[k])
                    {
                        if (pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value))
                        {
                            LockCookie lc = rwLock.UpgradeToWriterLock(Timeout.Infinite);
                            bool result = false;
                            try
                            {
                                result = map[k].Remove(pair);
                                if (map[k].Count == 0) map.Remove(k);
                            }
                            finally { rwLock.DowngradeFromWriterLock(ref lc); }
                            return result;
                        }
                    }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<MessageContent,T>> Membri di

        public IEnumerator<KeyValuePair<MessageContent, T>> GetEnumerator()
        {
            List<KeyValuePair<MessageContent, T>> listResult = new List<KeyValuePair<MessageContent, T>>();
            rwLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                {
                    foreach (KeyValuePair<MessageContent, T> pair in list)
                    {
                        listResult.Add(pair);
                    }
                }
            }
            finally { rwLock.ReleaseReaderLock(); }
            return listResult.GetEnumerator();
        }

        #endregion


        #region IEnumerable Membri di

        IEnumerator IEnumerable.GetEnumerator()
        {
            {
                ArrayList listResult = new ArrayList();
                rwLock.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                    {
                        foreach (KeyValuePair<MessageContent, T> pair in list)
                        {
                            listResult.Add(pair);
                        }
                    }
                }
                finally { rwLock.ReleaseReaderLock(); }
                return listResult.GetEnumerator();
            }
        }

        #endregion
    }


}

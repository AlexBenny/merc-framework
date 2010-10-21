using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace it.mintlab.mobilenet.mercframework
{
    class MCDictionary<T> : IDictionary<MessageContent,T>
    {

        private Dictionary<string, List<KeyValuePair<MessageContent,T>>> map;


        public MCDictionary()
        {
            map = new Dictionary<string, List<KeyValuePair<MessageContent,T>>>();
        }


        #region IDictionary<MessageContent,T> Membri di

        public void Add(MessageContent key, T value)
        {
            List<KeyValuePair<MessageContent,T>> list;
            string k = key.getKey();
            if (map.ContainsKey(k))
            {
                list = map[k];
            }
            else
            {
                list = new List<KeyValuePair<MessageContent, T>>();
                map.Add(key.getKey(), list);
            }
            list.Add(new KeyValuePair<MessageContent,T>(key,value));
        }

        public bool ContainsKey(MessageContent key)
        {
            string k = key.getKey();
            if (map.ContainsKey(k))
            {
                foreach (KeyValuePair<MessageContent,T> pair in map[k])
                {
                    if (pair.Key.Equals(key))
                        return true;
                }
            }
            return false;
        }

        public ICollection<MessageContent> Keys
        {
            get
            {
                ICollection<MessageContent> result = new List<MessageContent>();
                foreach (List<KeyValuePair<MessageContent,T>> list in map.Values)
                {
                    foreach (KeyValuePair<MessageContent,T> pair in list)
                    {
                        result.Add(pair.Key);
                    }
                }
                return result;
            }
        }

        public bool Remove(MessageContent key)
        {
            string k = key.getKey();
            if (map.ContainsKey(k))
            {
                foreach (KeyValuePair<MessageContent,T> pair in map[k])
                {
                    if (pair.Key.Equals(key))
                    {
                        bool result = map[k].Remove(pair);
                        if (map[k].Count == 0) map.Remove(k);
                        return result;
                    }
                }
            }
            return false;
        }

        public bool TryGetValue(MessageContent key, out T value)
        {
            value = default(T);
            string k = key.getKey();
            if (map.ContainsKey(k))
            {
                foreach (KeyValuePair<MessageContent,T> pair in map[k])
                {
                    if (pair.Key.Equals(key))
                    {
                        value = pair.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        public ICollection<T> Values
        {
            get
            {
                ICollection<T> result = new List<T>();
                foreach (List<KeyValuePair<MessageContent,T>> list in map.Values)
                {
                    foreach (KeyValuePair<MessageContent,T> pair in list)
                    {
                        result.Add(pair.Value);
                    }
                }
                return result;
            }
        }

        public T this[MessageContent key]
        {
            get
            {
                string k = key.getKey();
                if (map.ContainsKey(k))
                {
                    foreach (KeyValuePair<MessageContent,T> pair in map[k])
                    {
                        if (pair.Key.Equals(key))
                        {
                            return pair.Value;
                        }
                    }
                }
                throw new ArgumentException("No key found");
            }
            set
            {
                List<KeyValuePair<MessageContent,T>> list = new List<KeyValuePair<MessageContent,T>>();
                map.Add(key.getKey(), list);
                list.Add(new KeyValuePair<MessageContent,T>(key, value));
            }
        }

        #endregion

        #region ICollection<KeyValuePair<MessageContent,T>> Membri di

        public void Add(KeyValuePair<MessageContent, T> item)
        {
            List<KeyValuePair<MessageContent, T>> list;
            string k = item.Key.getKey();
            if (map.ContainsKey(k))
            {
                list = map[k];
            }
            else
            {
                list = new List<KeyValuePair<MessageContent, T>>();
                map.Add(item.Key.getKey(), list);
            }
            list.Add(new KeyValuePair<MessageContent, T>(item.Key, item.Value));
        }

        public void Clear()
        {
            map.Clear();
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
                foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                {
                    result += list.Count;
                }
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
            if (map.ContainsKey(k))
            {
                foreach (KeyValuePair<MessageContent, T> pair in map[k])
                {
                    if (pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value))
                    {
                        bool result = map[k].Remove(pair);
                        if (map[k].Count == 0) map.Remove(k);
                        return result;
                    }
                }
            }
            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair<MessageContent,T>> Membri di

        public IEnumerator<KeyValuePair<MessageContent, T>> GetEnumerator()
        {
            List<KeyValuePair<MessageContent, T>> listResult = new List<KeyValuePair<MessageContent, T>>();
            foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
            {
                foreach (KeyValuePair<MessageContent, T> pair in list)
                {
                    listResult.Add(pair);
                }
            }
            return listResult.GetEnumerator();
        }

        #endregion


        #region IEnumerable Membri di

        IEnumerator IEnumerable.GetEnumerator()
        {
            {
                ArrayList listResult = new ArrayList();
                foreach (List<KeyValuePair<MessageContent, T>> list in map.Values)
                {
                    foreach (KeyValuePair<MessageContent, T> pair in list)
                    {
                        listResult.Add(pair);
                    }
                }
                return listResult.GetEnumerator();
            }
        }

        #endregion
    }


}

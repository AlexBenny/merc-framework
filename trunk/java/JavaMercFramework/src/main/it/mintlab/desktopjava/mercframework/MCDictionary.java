package it.mintlab.desktopjava.mercframework;

import java.util.*;
import java.util.concurrent.locks.ReentrantReadWriteLock;

	public class MCDictionary<T> implements java.util.Map<MessageContent,T>
	{
		private ReentrantReadWriteLock rwLock = new ReentrantReadWriteLock();
		private HashMap<String, ArrayList<KeyValuePair<T>>> map;

		public MCDictionary()
		{
			map = new HashMap<String, ArrayList<KeyValuePair<T>>>();
		}

		/**
		 * @see java.util.Map#containsKey(java.lang.Object)
		 */
		public boolean containsKey(Object key) {
			String k = ((MessageContent)key).getKey();
			rwLock.readLock().lock();
            try
            {
            	if (map.containsKey(k))
	            {
					for (KeyValuePair<T> pair : map.get(k))
					{
						if (pair.getKey().equals(key))
						{
							return true;
						}
					}
				}
            }finally { rwLock.readLock().unlock(); }
            return false;
		}

		/**
		 * @see java.util.Map#containsValue(java.lang.Object)
		 */
		public boolean containsValue(Object value) {
			rwLock.readLock().lock();
			try{
				for (ArrayList<KeyValuePair<T>> list : map.values())
				{
					for (KeyValuePair<T> pair : list)
					{
						if(pair.getValue().equals(value)){
							return true;
						}
					}
				}
				return false;
			}finally { rwLock.readLock().unlock(); }
		}

		/**
		 * TODO
		 */
		public Set<Map.Entry<MessageContent, T>> entrySet() {
			return null;
			/*Set<KeyValuePair<T>> result = new HashSet<KeyValuePair<T>>();
			for (ArrayList<KeyValuePair<T>> list : map.values())
			{
				for (KeyValuePair<T> pair : list)
				{
					result.add(pair);
				}
			}
			return result;*/
		}


		/**
		 * @see java.util.Map#get(java.lang.Object)
		 */
		public T get(Object key) {
			String k = ((MessageContent)key).getKey();
			rwLock.readLock().lock();
			try{
				if (map.containsKey(k))
				{
					for (KeyValuePair<T> pair : map.get(k))
					{
						if (pair.getKey().equals(key))
						{
							return pair.getValue();
						}
					}
				}
				return null;
			}finally { rwLock.readLock().unlock(); }
		}


		/**
		 * @see java.util.Map#isEmpty()
		 */
		public boolean isEmpty() {
			rwLock.readLock().lock();
			try{
				return map.isEmpty();
			}finally { rwLock.readLock().unlock(); }
		}

		/**
		 * @see java.util.Map#keySet()
		 */
		public Set<MessageContent> keySet() {
			Set<MessageContent> result = new HashSet<MessageContent>();
			rwLock.readLock().lock();
            try
            {
				for (ArrayList<KeyValuePair<T>> list : map.values())
				{
					for (KeyValuePair<T> pair : list)
					{
						result.add(pair.getKey());
					}
				}
            }finally { rwLock.readLock().unlock(); }
			return result;
		}

		/**
		 * @see java.util.Map#put(java.lang.Object, java.lang.Object)
		 */
		public T put(MessageContent key, T value) {
			
			T oldValue = null;
			if(this.containsKey(key)){
				oldValue = this.get(key);
			}
			
			ArrayList<KeyValuePair<T>> list;
			String k = key.getKey();
			rwLock.readLock().lock();
            try
            {
				if (map.containsKey(k))
				{
					list = map.get(k);
				}
				else
				{
					list = new ArrayList<KeyValuePair<T>>();
					rwLock.readLock().unlock(); // must unlock first to obtain writelock
					rwLock.writeLock().lock();
					try{ 
						map.put(key.getKey(), list); 
					}finally { 
						rwLock.readLock().lock(); // reacquire read without giving up write lock
						rwLock.writeLock().unlock(); // unlock write, still hold read
					}
				}
				list.add(new KeyValuePair<T>(key, value));
            }
            finally { rwLock.readLock().unlock(); }
            
			return oldValue;
		}

		/**
		 * Not implementable
		 */
		public void putAll(Map<? extends MessageContent, ? extends T> t) {
			// TODO Auto-generated method stub
		}

		/**
		 * @see java.util.Map#remove(java.lang.Object)
		 */
		public T remove(Object key) {
			String k = ((MessageContent)key).getKey();
			rwLock.readLock().lock();
			try{
				if (map.containsKey(k))
				{
					for (KeyValuePair<T> pair : map.get(k))
					{
						if (pair.getKey().equals(key))
						{
							rwLock.readLock().unlock(); // must unlock first to obtain writelock
							rwLock.writeLock().lock();
							try{
								T ret = pair.getValue();
								boolean result = map.get(k).remove(pair);
								if (map.get(k).isEmpty())
								{
									map.remove(k);
								}
								if(result)
									return ret;	// return removed T value
								else
									return null;
							}finally { 
								rwLock.readLock().lock(); // reacquire read without giving up write lock
								rwLock.writeLock().unlock(); // unlock write, still hold read
							}
						}
					}
				}
			}finally { rwLock.readLock().unlock(); }
			return null;
		}


		/**
		 * @see java.util.Map#size()
		 */
		public int size() {
			int result = 0;
			rwLock.readLock().lock();
			try{
				for (ArrayList<KeyValuePair<T>> list : map.values())
				{
					result += list.size();
				}
				return result;
			}finally { rwLock.readLock().unlock(); }
		}


		/**
		 * @see java.util.Map#values()
		 */
		public Collection<T> values() {
			java.util.Collection<T> result = new java.util.ArrayList<T>();
			rwLock.readLock().lock();
	            try{
				for (ArrayList<KeyValuePair<T>> list : map.values())
				{
					for (KeyValuePair<T> pair : list)
					{
						result.add(pair.getValue());
					}
				}
				return result;
	        }finally { rwLock.readLock().unlock(); }
		}
		
		/**
		 * @see java.util.Map#clear()
		 */
		public void clear() {
			rwLock.writeLock().lock();
			try{
				map.clear();
			}finally { rwLock.writeLock().unlock(); }
		}
		
		
		/**
		 * Inner Class that represent a Key-Value Pair
		 * @param <V>
		 */
		private class KeyValuePair<V>
		{
			private MessageContent key;
			private V value;
			
			public KeyValuePair(MessageContent key, V value){
				this.key = key;
				this.value = value;
			}
			
			/**
			 * @param key the key to set
			 */
			public void setKey(MessageContent key) {
				this.key = key;
			}
			
			/**
			 * @return the key
			 */
			public MessageContent getKey() {
				return key;
			}

			/**
			 * @param value the value to set
			 */
			public void setValue(V value) {
				this.value = value;
			}

			/**
			 * @return the value
			 */
			public V getValue() {
				return value;
			}
			
		}
		
		/* FROM .NET
		public final java.util.Collection<MessageContent> getKeys()
		{
			java.util.Collection<MessageContent> result = new java.util.ArrayList<MessageContent>();
			for (java.util.ArrayList<java.util.Map.Entry<MessageContent,T>> list : map.values())
			{
				for (java.util.Map.Entry<MessageContent,T> pair : list)
				{
					result.add(pair.getKey());
				}
			}
			return result;
		}*/

		/* FROM .NET
 		public final boolean TryGetValue(MessageContent key, RefObject<T> value)
		 
		{
			value.argvalue = null;
			String k = key.getKey();
			if (map.containsKey(k))
			{
				for (java.util.Map.Entry<MessageContent,T> pair : map.get(k))
				{
					if (pair.getKey().equals(key))
					{
						value.argvalue = pair.getValue();
						return true;
					}
				}
			}
			return false;
		}*/

		/* FROM .NET
		public final void setItem(MessageContent key, T value)
		{
			java.util.ArrayList<java.util.Map.Entry<MessageContent,T>> list = new java.util.ArrayList<java.util.Map.Entry<MessageContent,T>>();
			map.put(key.getKey(), list);
			list.add(new KeyValuePair<MessageContent,T>(key, value));
		}*/
		
		/* FROM .NET
		public final void add(java.util.Map.Entry<MessageContent, T> item)
		{
			java.util.ArrayList<KeyValuePair<T>> list;
			String k = item.getKey().getKey();
			if (map.containsKey(k))
			{
				list = map.get(k);
			}
			else
			{
				list = new java.util.ArrayList<KeyValuePair<T>>();
				map.put(item.getKey().getKey(), list);
			}
			list.add(KeyValuePair<T>(item.getKey(), item.getValue()));
		}*/

		/* FROM .NET
		public final void clear()
		{
			map.clear();
		}*/

		/* FROM .NET
		public final void CopyTo(java.util.Map.Entry<MessageContent, T>[] array, int arrayIndex)
		{
			throw new RuntimeException("The method or operation is not implemented.");
		}*/

		/* FROM .NET
		public final boolean getIsReadOnly()
		{
			return false;
		}*/

		/* FROM .NET
		public final java.util.Iterator<java.util.Map.Entry<MessageContent, T>> GetEnumerator()
		{
			java.util.ArrayList<java.util.Map.Entry<MessageContent, T>> listResult = new java.util.ArrayList<java.util.Map.Entry<MessageContent, T>>();
			for (java.util.ArrayList<java.util.Map.Entry<MessageContent, T>> list : map.values())
			{
				for (java.util.Map.Entry<MessageContent, T> pair : list)
				{
					listResult.add(pair);
				}
			}
			return listResult.iterator();
		}*/

		/* FROM .NET
		private java.util.Iterator GetEnumerator()
		{
			{
				java.util.ArrayList listResult = new java.util.ArrayList();
				for (java.util.ArrayList<java.util.Map.Entry<MessageContent, T>> list : map.values())
				{
					for (java.util.Map.Entry<MessageContent, T> pair : list)
					{
						listResult.add(pair);
					}
				}
				return listResult.iterator();
			}
		}*/
		

	}
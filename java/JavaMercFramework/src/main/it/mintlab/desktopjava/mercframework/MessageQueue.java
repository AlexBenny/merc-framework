package it.mintlab.desktopjava.mercframework;

	public class MessageQueue
	{
		private Object lockToken;
		private java.util.ArrayList<Message> generalQueue;
		private java.util.HashMap<MessageContent, java.util.ArrayList<Message>> filteredQueues;

		public MessageQueue()
		{
			lockToken = new Object(); //Needed for locking
			generalQueue = new java.util.ArrayList<Message>();
			filteredQueues = new java.util.HashMap<MessageContent, java.util.ArrayList<Message>>();
		}

		/** 
		 Enqueue a message
		 
		 <param id="message">
		*/
		public final void enqueue(Message message)
		{
			synchronized (lockToken)
			{
				generalQueue.add(message);
				MessageContent key = message.getContent();
				if (filteredQueues.containsKey(key))
				{
					filteredQueues.get(key).add(message);
				}
				else
				{
					java.util.ArrayList<Message> l = new java.util.ArrayList<Message>();
					l.add(message);
					filteredQueues.put(key, l);
				}
			}
		}

		/** 
		 Dequeue a message
		 
		 @return 
		 * @throws Exception 
		*/
		public final Message dequeue() throws Exception
		{
			synchronized (lockToken)
			{
				if (generalQueue.isEmpty())
				{
					throw new Exception("MessageQueue empty");
				}
				Message result = generalQueue.get(0);
				generalQueue.remove(0);
				MessageContent key = result.getContent();
				filteredQueues.get(key).remove(0);
				if(filteredQueues.size() == 0)
					filteredQueues.remove(key);
				return result;
			}
		}

		/** 
		 Dequeue a message of a particular filter
		 
		 @return 
		 * @throws Exception 
		*/
		public final Message dequeue(MessageContent filter) throws Exception
		{
			synchronized (lockToken)
			{
				if (!filteredQueues.containsKey(filter))
				{
					throw new Exception("MessageQueue empty for content "+filter);
				}
				java.util.ArrayList<Message> l = filteredQueues.get(filter);
				Message result = l.get(0);
				l.remove(0);
				if(filteredQueues.size() == 0)
					filteredQueues.remove(filter);
				generalQueue.remove(result);
				return result;
			}
		}

		/** 
		 Returns true if this queue contains the specified filter
		 
		 <param id="filter">filter whose presence in this queue is to be tested
		 @return true if the specified element is present; false otherwise
		*/
		public final boolean contains(MessageContent filter)
		{
			return filteredQueues.containsKey(filter);
		}


		/** 
		 Clear all messages
		 
		*/
		public final void clear()
		{
			synchronized (lockToken)
			{
				generalQueue.clear();
				filteredQueues.clear();
			}
		}

		/** 
		 Clear all messages of a filter
		 
		 <param id="category">
		*/
		public final void clear(MessageContent filter)
		{
			synchronized (lockToken)
			{
				if (!filteredQueues.containsKey(filter))
				{
					return;
				}
				java.util.ArrayList<Message> l = filteredQueues.get(filter);
				filteredQueues.remove(filter);
				for (Message msg : l)
				{
					generalQueue.remove(msg);
				}
			}
		}

		/** 
		 Return True if queue is empty
		 
		 @return 
		*/
		public final boolean isEmpty()
		{
			if (generalQueue.isEmpty())
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	}
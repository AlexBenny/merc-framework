using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.mercframework
{
    internal class MessageQueue
    {
        private object lockToken;
        private List<Message> generalQueue;
        private Dictionary<MessageContent, List<Message>> filteredQueues;

        public MessageQueue()
        {
            lockToken = new object(); //Needed for locking
            generalQueue = new List<Message>();
            filteredQueues = new Dictionary<MessageContent, List<Message>>();
        }

        /// <summary>
        /// Enqueue a message
        /// </summary>
        /// <param id="message"></param>
        public void enqueue(Message message)
        {
            lock (lockToken)
            {
                generalQueue.Add(message);
                MessageContent key = message.getContent();
                if (filteredQueues.ContainsKey(key))
                {
                    filteredQueues[key].Add(message);
                }
                else
                {
                    List<Message> l = new List<Message>();
                    l.Add(message);
                    filteredQueues.Add(key, l);
                }
            }
        }

        /// <summary>
        /// Dequeue a message
        /// </summary>
        /// <returns></returns>
        public Message dequeue()
        {
            lock (lockToken)
            {
                if (generalQueue.Count == 0)
                    throw new InvalidOperationException("MessageQueue empty");
                Message result = generalQueue[0];
                generalQueue.RemoveAt(0);
                MessageContent key = result.getContent();
                filteredQueues[key].RemoveAt(0);
                if (filteredQueues.Count == 0)
                    filteredQueues.Remove(key);
                return result;
            }
        }

        /// <summary>
        /// Dequeue a message of a particular filter
        /// </summary>
        /// <returns></returns>
        public Message dequeue(MessageContent filter)
        {
            lock (lockToken)
            {
                if (!filteredQueues.ContainsKey(filter))
                    throw new InvalidOperationException("MessageQueue empty for content "+filter);
                List<Message> l = filteredQueues[filter];
                Message result = l[0];
                l.RemoveAt(0);
                if (filteredQueues.Count == 0)
                    filteredQueues.Remove(filter);
                generalQueue.Remove(result);
                return result;
            }
        }

        /// <summary>
        /// Returns true if this queue contains the specified filter
        /// </summary>
        /// <param id="filter">filter whose presence in this queue is to be tested</param>
        /// <returns>true if the specified element is present; false otherwise</returns>
        public bool contains(MessageContent filter)
        {
            return filteredQueues.ContainsKey(filter);
        }


        /// <summary>
        /// Clear all messages
        /// </summary>
        public void clear()
        {
            lock (lockToken)
            {
                generalQueue.Clear();
                filteredQueues.Clear();
            }
        }

        /// <summary>
        /// Clear all messages of a filter declared
        /// </summary>
        /// <param id="filter"></param>
        public void clear(MessageContent filter)
        {
            lock (lockToken)
            {
                if (!filteredQueues.ContainsKey(filter)) return;
                List<Message> l = filteredQueues[filter];
                filteredQueues.Remove(filter);
                foreach (Message msg in l)
                {
                    generalQueue.Remove(msg);
                }
            }
        }

        /// <summary>
        /// Return True if queue is empty
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            if (generalQueue.Count == 0)
                return true;
            else
                return false;
        }

    }
}

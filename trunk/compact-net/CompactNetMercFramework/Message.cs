using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.mobilenet.tupleengine;

namespace it.mintlab.mobilenet.mercframework
{
    public class Message
    {
        // Broadcast const
        public const string BROADCAST_LOCAL = "#LOCAL";
        public const string BROADCAST_REMOTE = "#REMOTE";
        public const string BROADCAST_ALL = "#ALL";
        //public const string BROADCAST_REMOTE_MULTIHOP = "#REMOTE_MULTIHOP";

        private DateTime timestamp;
        private string sender;
        private string recipient;
        private MessageContent content;

        public Message(string sender, string recipient, MessageContent content)
        {
            this.sender = sender;
            if (recipient == null)
                this.recipient = BROADCAST_LOCAL;
            else
                this.recipient = recipient;
            this.content = content;
        }

        public string getSender()
        {
            return sender;
        }

        public string getRecipient()
        {
            return recipient;
        }

        public MessageContent getContent()
        {
            return content;
        }

        public void setTimestamp(DateTime timestamp)
        {
            this.timestamp = timestamp;
        }

        public DateTime getTimestamp()
        {
            return timestamp;
        }

        public override bool Equals(object obj)
        {
            Message msg = (Message)obj;
            return sender.Equals(msg.sender) &&
                recipient.Equals(msg.recipient) &&
                content.Equals(msg.content);
        }

        public override string ToString()
        {
            return "<" + sender + "," + recipient + "," + content.getCategory() + "," + content.getTuple() + ">";
        }

    }
}

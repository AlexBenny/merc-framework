package it.mintlab.desktopjava.mercframework;

import it.mintlab.desktopjava.tupleengine.*;

	public class Message
	{
		// Broadcast const
        public static final String BROADCAST_LOCAL = "#LOCAL";
        public static final String BROADCAST_REMOTE = "#REMOTE";
        public static final String BROADCAST_ALL = "#ALL";
        
		private java.util.Date timestamp = new java.util.Date();
		private String sender;
		private String recipient;
		private MessageContent content;

		public Message(String sender, String recipient, MessageContent content)
		{
			this.sender = sender;
			if (recipient == null)
				this.recipient = BROADCAST_LOCAL;
			else
			    this.recipient = recipient;
			this.content = content;
		}

		public final String getSender()
		{
			return sender;
		}

		public final String getRecipient()
		{
			return recipient;
		}

		public final void setTimestamp(java.util.Date timestamp)
		{
			this.timestamp = timestamp;
		}

		public final java.util.Date getTimestamp()
		{
			return timestamp;
		}

		public final MessageContent getContent()
		{
			return content;
		}

		@Override
		public String toString()
		{
			return "<" + sender + "," + recipient + "," + content.getCategory() + "," + content.getTuple() + ">";
		}

		@Override
		public boolean equals(Object obj)
		{
			Message msg = (Message)obj;
			return sender.equals(msg.sender) && recipient.equals(msg.recipient) && content == msg.content;
		}
	}
package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.*;

	/**
	 * This Merc for testing sends hello to receiver and broadcasts a question
	 */
	public class SenderMerc extends Merc
	{
		@MessageBinding(message  = "init")
		public final void init()
		{
			dispatcher.deliverMessage("Receiver", "hello");
		}
		
		@MessageBinding(message = "hello")
        public void hello()
        {
        }

		@MessageBinding(message = "question(_0)")
		public void question(String msg)
		{
			dispatcher.deliverMessage("Receiver", "response('Not bad..')");
		    dispatcher.deliverMessage(Message.BROADCAST_REMOTE, "question('What are you doing, guys?')");
		}

		@MessageBinding(message  = "response(_0)")
		public final void response(String msg)
		{
			dispatcher.deliverMessage(Message.BROADCAST_ALL, "bye");
			dispatcher.killMerc();
		}

	}
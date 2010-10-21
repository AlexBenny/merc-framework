package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.*;

	/**
	 * This Merc waits for Sender hello and responding to it
	 */
	public class ReceiverMerc extends Merc
	{

		@MessageBinding(message  = "init")
		public void init()
		{
			dispatcher.logInfo("Waiting for friend..");
		}

		@MessageBinding(message  = "hello")
		public void hello()
		{
			dispatcher.deliverMessage("Sender", "question(\"How are you?\")");
		}

		@MessageBinding(message  = "response(_0)")
		public void response(String msg)
		{
		}
		
		@MessageBinding(message = "question(_0)")
        public void question(String msg)
        {
            dispatcher.deliverMessage("Sender", "response('I do not know')");
        }

		@MessageBinding(message = "bye")
        public void bye()
        {
            dispatcher.killMerc();
        }
	}
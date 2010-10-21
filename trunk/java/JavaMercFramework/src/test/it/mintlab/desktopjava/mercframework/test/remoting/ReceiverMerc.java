package it.mintlab.desktopjava.mercframework.test.remoting;

import it.mintlab.desktopjava.mercframework.*;

	@MessageBinding(message  = "init")
	public class ReceiverMerc extends Merc
	{

		@MessageBinding(message  = "init")
		public void init()
		{
			dispatcher.logInfo("Waiting for friend..");
		}

		@MessageBinding(message  = "hello(_0)")
		public void hello(String msg)
		{
			dispatcher.logInfo("Hello from remote: " + msg);
			//dispatcher.deliverMessage("Sender", "response(fineAndYou)");
		}

		@MessageBinding(message  = "response(_0)")
		public void response(String msg)
		{
			dispatcher.logInfo("Response from remote: " + msg);
			dispatcher.killMerc();
		}
	}
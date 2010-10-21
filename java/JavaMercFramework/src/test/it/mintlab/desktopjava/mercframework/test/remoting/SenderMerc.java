package it.mintlab.desktopjava.mercframework.test.remoting;

import it.mintlab.desktopjava.mercframework.*;

	public class SenderMerc extends Merc
	{
		@MessageBinding(message  = "init")
		public final void init()
		{
			dispatcher.deliverMessage("Receiver", "hello(howAreYou)");
			//dispatcher.deliverMessage("Receiver", "hello(boy)");
		}

		@MessageBinding(message  = "response(_0)")
		public final void response(String msg)
		{
			dispatcher.logInfo("Response from remote: " + msg);
			dispatcher.deliverMessage("Receiver", "response(notBad)");
			dispatcher.killMerc();
		}

	}
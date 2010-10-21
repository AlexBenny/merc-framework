package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.*;

	public class MultipleStateFilterMerc extends Merc
	{
		private static final String STATE_A = "STATE_A";
		private static final String STATE_B = "STATE_B";
		private static final String STATE_C = "STATE_C";


		@MessageBinding(message = "init")
		public final void init()
		{
			setState(STATE_A);
			dispatcher.deliverMessage("TestTupleWithUpperCase");
		}

		@MessageBinding(message = "TestTupleWithUpperCase", states = { STATE_A, STATE_B })
		public final void f1()
		{
			if (getState().equals(STATE_A))
			{
				setState(STATE_B);
				dispatcher.deliverMessage("TestTupleWithUpperCase");
			}
			if (getState().equals(STATE_B))
			{
				setState(STATE_C);
				dispatcher.deliverMessage("testFilterSender");
			}
		}

		@MessageBinding(message = "testFilterSender", senderParam = "_sender", senders = {"Test"})
		public final void f2(String sender)
		{
			System.out.println("OK... " + sender);
		}

	}
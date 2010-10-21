package it.mintlab.desktopjava.mercframework.test;

import it.mintlab.desktopjava.mercframework.*;

	public class StateChangeReactionMerc extends Merc
	{
		private static final String STATE_A = "STATE_A";
		private static final String STATE_B = "STATE_B";
		//private static final String STATE_C = "STATE_C";

		@MessageBinding(message = "init")
		public final void init()
		{
			setState(STATE_A);
		}

		@StateBinding(state = STATE_A)
		public final void stateA()
		{
			System.out.println("State A");
			setState(STATE_B);
		}

		@StateBinding(state = STATE_B)
		public final void stateB()
		{
			System.out.println("State B");
			setState(STATE_B);
		}

	}